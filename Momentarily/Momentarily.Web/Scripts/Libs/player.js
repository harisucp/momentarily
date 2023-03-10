import './lib/compatibility-check';

import 'es6-collections';
import Promise from 'native-promise-only';

import { storeCallback, getCallbacks, removeCallback, swapCallbacks } from './lib/callbacks';
import { getMethodName, isDomElement, isVimeoUrl, getVimeoUrl } from './lib/functions';
import { getOEmbedParameters, getOEmbedData, createEmbed, initializeEmbeds } from './lib/embed';
import { parseMessageData, postMessage, processData } from './lib/postmessage';

const playerMap = new WeakMap();
const readyMap = new WeakMap();

class Player {
     /**
     * Create a Player.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {(HTMLIFrameElement|HTMLElement|string|jQuery)} element A reference to the Vimeo
     *        player iframe, and id, or a jQuery object.
     * @param {object} [options] oEmbed parameters to use when creating an embed in the element.
     * @return {Player}
     */
    constructor(element, options = {}) {
        /* global jQuery */
        if (window.jQuery && element instanceof jQuery) {
            if (element.length > 1 && window.console && console.warn) {
                console.warn('A jQuery object with multiple elements was passed, using the first element.');
            }

            element = element[0];
        }

        // Find an element by ID
        if (typeof element === 'string') {
            element = document.getElementById(element);
        }

        // Not an element!
        if (!isDomElement(element)) {
            throw new TypeError('You must pass either a valid element or a valid id.');
        }

        // Already initialized an embed in this div, so grab the iframe
        if (element.nodeName !== 'IFRAME') {
            const iframe = element.querySelector('iframe');

            if (iframe) {
                element = iframe;
            }
        }

        // iframe url is not a Vimeo url
        if (element.nodeName === 'IFRAME' && !isVimeoUrl(element.getAttribute('src') || '')) {
            throw new Error('The player element passed isn???t a Vimeo embed.');
        }

        // If there is already a player object in the map, return that
        if (playerMap.has(element)) {
            return playerMap.get(element);
        }

        this.element = element;
        this.origin = '*';

        const readyPromise = new Promise((resolve, reject) => {
            const onMessage = (event) => {
                if (!isVimeoUrl(event.origin) || this.element.contentWindow !== event.source) {
                    return;
                }

                if (this.origin === '*') {
                    this.origin = event.origin;
                }

                const data = parseMessageData(event.data);
                const isReadyEvent = 'event' in data && data.event === 'ready';
                const isPingResponse = 'method' in data && data.method === 'ping';

                if (isReadyEvent || isPingResponse) {
                    this.element.setAttribute('data-ready', 'true');
                    resolve();
                    return;
                }

                processData(this, data);
            };

            if (window.addEventListener) {
                window.addEventListener('message', onMessage, false);
            }
            else if (window.attachEvent) {
                window.attachEvent('onmessage', onMessage);
            }

            if (this.element.nodeName !== 'IFRAME') {
                const params = getOEmbedParameters(element, options);
                const url = getVimeoUrl(params);

                getOEmbedData(url, params).then((data) => {
                    const iframe = createEmbed(data, element);
                    this.element = iframe;
                    swapCallbacks(element, iframe);

                    return data;
                }).catch((error) => reject(error));
            }
        });

        // Store a copy of this Player in the map
        readyMap.set(this, readyPromise);
        playerMap.set(this.element, this);

        // Send a ping to the iframe so the ready promise will be resolved if
        // the player is already ready.
        if (this.element.nodeName === 'IFRAME') {
            postMessage(this, 'ping');
        }

        return this;
    }

    /**
    * Call a function when the player is initialized.
    *
    * @author Brad Dougherty <brad@vimeo.com>
    * @param {function} onFulfilled Function to be called when the player
    *        is initialized.
    * @param {function} [onRejected] Function to be called if
    *        there is an error initializing the player.
    * @return {Promise}
    */
    then(onFulfilled, onRejected = () => {}) {
        return this.ready().then(onFulfilled, onRejected);
    }

    /**
     * Get a promise for a method.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} name The API method to call.
     * @param {Object} [args={}] Arguments to send via postMessage.
     * @return {Promise}
     */
    callMethod(name, args = {}) {
        return new Promise((resolve, reject) => {
            // We are storing the resolve/reject handlers to call later, so we
            // can???t return here.
            // eslint-disable-next-line promise/always-return
            return this.ready().then(() => {
                storeCallback(this, name, {
                    resolve,
                    reject
                });

                postMessage(this, name, args);
            });
        });
    }

    /**
     * Get a promise for the value of a player property.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} name The property name
     * @return {Promise}
     */
    get(name) {
        return new Promise((resolve, reject) => {
            name = getMethodName(name, 'get');

            // We are storing the resolve/reject handlers to call later, so we
            // can???t return here.
            // eslint-disable-next-line promise/always-return
            return this.ready().then(() => {
                storeCallback(this, name, {
                    resolve,
                    reject
                });

                postMessage(this, name);
            });
        });
    }

    /**
     * Get a promise for setting the value of a player property.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} name The API method to call.
     * @param {mixed} value The value to set.
     * @return {Promise}
     */
    set(name, value) {
        return Promise.resolve(value).then((val) => {
            name = getMethodName(name, 'set');

            if (val === undefined || val === null) {
                throw new TypeError('There must be a value to set.');
            }

            return this.ready().then(() => {
                return new Promise((resolve, reject) => {
                    storeCallback(this, name, {
                        resolve,
                        reject
                    });

                    postMessage(this, name, val);
                });
            });
        });
    }

    /**
     * Add an event listener for the specified event. Will call the
     * callback with a single parameter, `data`, that contains the data for
     * that event.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} eventName The name of the event.
     * @param {function(*)} callback The function to call when the event fires.
     * @return {void}
     */
    on(eventName, callback) {
        if (!eventName) {
            throw new TypeError('You must pass an event name.');
        }

        if (!callback) {
            throw new TypeError('You must pass a callback function.');
        }

        if (typeof callback !== 'function') {
            throw new TypeError('The callback must be a function.');
        }

        const callbacks = getCallbacks(this, `event:${eventName}`);
        if (callbacks.length === 0) {
            this.callMethod('addEventListener', eventName).catch(() => {
                // Ignore the error. There will be an error event fired that
                // will trigger the error callback if they are listening.
            });
        }

        storeCallback(this, `event:${eventName}`, callback);
    }

    /**
     * Remove an event listener for the specified event. Will remove all
     * listeners for that event if a `callback` isn???t passed, or only that
     * specific callback if it is passed.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} eventName The name of the event.
     * @param {function} [callback] The specific callback to remove.
     * @return {void}
     */
    off(eventName, callback) {
        if (!eventName) {
            throw new TypeError('You must pass an event name.');
        }

        if (callback && typeof callback !== 'function') {
            throw new TypeError('The callback must be a function.');
        }

        const lastCallback = removeCallback(this, `event:${eventName}`, callback);

        // If there are no callbacks left, remove the listener
        if (lastCallback) {
            this.callMethod('removeEventListener', eventName).catch((e) => {
                // Ignore the error. There will be an error event fired that
                // will trigger the error callback if they are listening.
            });
        }
    }

    /**
     * A promise to load a new video.
     *
     * @promise LoadVideoPromise
     * @fulfill {number} The video with this id successfully loaded.
     * @reject {TypeError} The id was not a number.
     */
    /**
     * Load a new video into this embed. The promise will be resolved if
     * the video is successfully loaded, or it will be rejected if it could
     * not be loaded.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {number} id The id of the video.
     * @return {LoadVideoPromise}
     */
    loadVideo(id) {
        return this.callMethod('loadVideo', id);
    }

    /**
     * A promise to perform an action when the Player is ready.
     *
     * @todo document errors
     * @promise LoadVideoPromise
     * @fulfill {void}
     */
    /**
     * Trigger a function when the player iframe has initialized. You do not
     * need to wait for `ready` to trigger to begin adding event listeners
     * or calling other methods.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {ReadyPromise}
     */
    ready() {
        const readyPromise = readyMap.get(this);
        return Promise.resolve(readyPromise);
    }

    /**
     * A representation of a text track on a video.
     *
     * @typedef {Object} VimeoTextTrack
     * @property {string} language The ISO language code.
     * @property {string} kind The kind of track it is (captions or subtitles).
     * @property {string} label The human???readable label for the track.
     */
    /**
     * A promise to enable a text track.
     *
     * @promise EnableTextTrackPromise
     * @fulfill {VimeoTextTrack} The text track that was enabled.
     * @reject {InvalidTrackLanguageError} No track was available with the
     *         specified language.
     * @reject {InvalidTrackError} No track was available with the specified
     *         language and kind.
     */
    /**
     * Enable the text track with the specified language, and optionally the
     * specified kind (captions or subtitles).
     *
     * When set via the API, the track language will not change the viewer???s
     * stored preference.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} language The two???letter language code.
     * @param {string} [kind] The kind of track to enable (captions or subtitles).
     * @return {EnableTextTrackPromise}
     */
    enableTextTrack(language, kind) {
        if (!language) {
            throw new TypeError('You must pass a language.');
        }

        return this.callMethod('enableTextTrack', {
            language,
            kind
        });
    }

    /**
     * A promise to disable the active text track.
     *
     * @promise DisableTextTrackPromise
     * @fulfill {void} The track was disabled.
     */
    /**
     * Disable the currently-active text track.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {DisableTextTrackPromise}
     */
    disableTextTrack() {
        return this.callMethod('disableTextTrack');
    }

    /**
     * A promise to pause the video.
     *
     * @promise PausePromise
     * @fulfill {void} The video was paused.
     */
    /**
     * Pause the video if it???s playing.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {PausePromise}
     */
    pause() {
        return this.callMethod('pause');
    }

    /**
     * A promise to play the video.
     *
     * @promise PlayPromise
     * @fulfill {void} The video was played.
     */
    /**
     * Play the video if it???s paused. **Note:** on iOS and some other
     * mobile devices, you cannot programmatically trigger play. Once the
     * viewer has tapped on the play button in the player, however, you
     * will be able to use this function.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {PlayPromise}
     */
    play() {
        return this.callMethod('play');
    }

    /**
     * A promise to unload the video.
     *
     * @promise UnloadPromise
     * @fulfill {void} The video was unloaded.
     */
    /**
     * Return the player to its initial state.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {UnloadPromise}
     */
    unload() {
        return this.callMethod('unload');
    }

    /**
     * A promise to get the autopause behavior of the video.
     *
     * @promise GetAutopausePromise
     * @fulfill {boolean} Whether autopause is turned on or off.
     * @reject {UnsupportedError} Autopause is not supported with the current
     *         player or browser.
     */
    /**
     * Get the autopause behavior for this player.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetAutopausePromise}
     */
    getAutopause() {
        return this.get('autopause');
    }

    /**
     * A promise to set the autopause behavior of the video.
     *
     * @promise SetAutopausePromise
     * @fulfill {boolean} Whether autopause is turned on or off.
     * @reject {UnsupportedError} Autopause is not supported with the current
     *         player or browser.
     */
    /**
     * Enable or disable the autopause behavior of this player.
     *
     * By default, when another video is played in the same browser, this
     * player will automatically pause. Unless you have a specific reason
     * for doing so, we recommend that you leave autopause set to the
     * default (`true`).
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {boolean} autopause
     * @return {SetAutopausePromise}
     */
    setAutopause(autopause) {
        return this.set('autopause', autopause);
    }

    /**
     * A promise to get the color of the player.
     *
     * @promise GetColorPromise
     * @fulfill {string} The hex color of the player.
     */
    /**
     * Get the color for this player.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetColorPromise}
     */
    getColor() {
        return this.get('color');
    }

    /**
     * A promise to set the color of the player.
     *
     * @promise SetColorPromise
     * @fulfill {string} The color was successfully set.
     * @reject {TypeError} The string was not a valid hex or rgb color.
     * @reject {ContrastError} The color was set, but the contrast is
     *         outside of the acceptable range.
     * @reject {EmbedSettingsError} The owner of the player has chosen to
     *         use a specific color.
     */
    /**
     * Set the color of this player to a hex or rgb string. Setting the
     * color may fail if the owner of the video has set their embed
     * preferences to force a specific color.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {string} color The hex or rgb color string to set.
     * @return {SetColorPromise}
     */
    setColor(color) {
        return this.set('color', color);
    }

    /**
     * A promise to get the current time of the video.
     *
     * @promise GetCurrentTimePromise
     * @fulfill {number} The current time in seconds.
     */
    /**
     * Get the current playback position in seconds.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetCurrentTimePromise}
     */
    getCurrentTime() {
        return this.get('currentTime');
    }

    /**
     * A promise to set the current time of the video.
     *
     * @promise SetCurrentTimePromise
     * @fulfill {number} The actual current time that was set.
     * @reject {RangeError} the time was less than 0 or greater than the
     *         video???s duration.
     */
    /**
     * Set the current playback position in seconds. If the player was
     * paused, it will remain paused. Likewise, if the player was playing,
     * it will resume playing once the video has buffered.
     *
     * You can provide an accurate time and the player will attempt to seek
     * to as close to that time as possible. The exact time will be the
     * fulfilled value of the promise.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {number} currentTime
     * @return {SetCurrentTimePromise}
     */
    setCurrentTime(currentTime) {
        return this.set('currentTime', currentTime);
    }

    /**
     * A promise to get the duration of the video.
     *
     * @promise GetDurationPromise
     * @fulfill {number} The duration in seconds.
     */
    /**
     * Get the duration of the video in seconds. It will be rounded to the
     * nearest second before playback begins, and to the nearest thousandth
     * of a second after playback begins.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetDurationPromise}
     */
    getDuration() {
        return this.get('duration');
    }

    /**
     * A promise to get the ended state of the video.
     *
     * @promise GetEndedPromise
     * @fulfill {boolean} Whether or not the video has ended.
     */
    /**
     * Get the ended state of the video. The video has ended if
     * `currentTime === duration`.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetEndedPromise}
     */
    getEnded() {
        return this.get('ended');
    }

    /**
     * A promise to get the loop state of the player.
     *
     * @promise GetLoopPromise
     * @fulfill {boolean} Whether or not the player is set to loop.
     */
    /**
     * Get the loop state of the player.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetLoopPromise}
     */
    getLoop() {
        return this.get('loop');
    }

    /**
     * A promise to set the loop state of the player.
     *
     * @promise SetLoopPromise
     * @fulfill {boolean} The loop state that was set.
     */
    /**
     * Set the loop state of the player. When set to `true`, the player
     * will start over immediately once playback ends.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {boolean} loop
     * @return {SetLoopPromise}
     */
    setLoop(loop) {
        return this.set('loop', loop);
    }

    /**
     * A promise to get the paused state of the player.
     *
     * @promise GetLoopPromise
     * @fulfill {boolean} Whether or not the video is paused.
     */
    /**
     * Get the paused state of the player.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetLoopPromise}
     */
    getPaused() {
        return this.get('paused');
    }

    /**
     * A promise to get the text tracks of a video.
     *
     * @promise GetTextTracksPromise
     * @fulfill {VimeoTextTrack[]} The text tracks associated with the video.
     */
    /**
     * Get an array of the text tracks that exist for the video.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetTextTracksPromise}
     */
    getTextTracks() {
        return this.get('textTracks');
    }

    /**
     * A promise to get the embed code for the video.
     *
     * @promise GetVideoEmbedCodePromise
     * @fulfill {string} The `<iframe>` embed code for the video.
     */
    /**
     * Get the `<iframe>` embed code for the video.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoEmbedCodePromise}
     */
    getVideoEmbedCode() {
        return this.get('videoEmbedCode');
    }

    /**
     * A promise to get the id of the video.
     *
     * @promise GetVideoIdPromise
     * @fulfill {number} The id of the video.
     */
    /**
     * Get the id of the video.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoIdPromise}
     */
    getVideoId() {
        return this.get('videoId');
    }

    /**
     * A promise to get the title of the video.
     *
     * @promise GetVideoTitlePromise
     * @fulfill {number} The title of the video.
     */
    /**
     * Get the title of the video.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoTitlePromise}
     */
    getVideoTitle() {
        return this.get('videoTitle');
    }

    /**
     * A promise to get the native width of the video.
     *
     * @promise GetVideoWidthPromise
     * @fulfill {number} The native width of the video.
     */
    /**
     * Get the native width of the currently???playing video. The width of
     * the highest???resolution available will be used before playback begins.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoWidthPromise}
     */
    getVideoWidth() {
        return this.get('videoWidth');
    }

    /**
     * A promise to get the native height of the video.
     *
     * @promise GetVideoHeightPromise
     * @fulfill {number} The native height of the video.
     */
    /**
     * Get the native height of the currently???playing video. The height of
     * the highest???resolution available will be used before playback begins.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoHeightPromise}
     */
    getVideoHeight() {
        return this.get('videoHeight');
    }

    /**
     * A promise to get the vimeo.com url for the video.
     *
     * @promise GetVideoUrlPromise
     * @fulfill {number} The vimeo.com url for the video.
     * @reject {PrivacyError} The url isn???t available because of the video???s privacy setting.
     */
    /**
     * Get the vimeo.com url for the video.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVideoUrlPromise}
     */
    getVideoUrl() {
        return this.get('videoUrl');
    }

    /**
     * A promise to get the volume level of the player.
     *
     * @promise GetVolumePromise
     * @fulfill {number} The volume level of the player on a scale from 0 to 1.
     */
    /**
     * Get the current volume level of the player on a scale from `0` to `1`.
     *
     * Most mobile devices do not support an independent volume from the
     * system volume. In those cases, this method will always return `1`.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @return {GetVolumePromise}
     */
    getVolume() {
        return this.get('volume');
    }

    /**
     * A promise to set the volume level of the player.
     *
     * @promise SetVolumePromise
     * @fulfill {number} The volume was set.
     * @reject {RangeError} The volume was less than 0 or greater than 1.
     */
    /**
     * Set the volume of the player on a scale from `0` to `1`. When set
     * via the API, the volume level will not be synchronized to other
     * players or stored as the viewer???s preference.
     *
     * Most mobile devices do not support setting the volume. An error will
     * *not* be triggered in that situation.
     *
     * @author Brad Dougherty <brad@vimeo.com>
     * @param {number} volume
     * @return {SetVolumePromise}
     */
    setVolume(volume) {
        return this.set('volume', volume);
    }
}

initializeEmbeds();

export default Player;
