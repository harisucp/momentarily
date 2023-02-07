angular.module('Apeek').service('LoadGoogleMapAPI', ['$window', '$q', function ($window, $q) {
    var self = this;
    
    var asyncUrl = '//maps.googleapis.com/maps/api/js?key=AIzaSyBP9cfi8Ngb5bgwFu253vGDaaTihNMGjXg&signed_in=false&libraries=places&language=en&callback=';
    var mapsDefer = $q.defer();
    
    $window.googleMapsInitialized = mapsDefer.resolve; // removed ()
    
    var asyncLoad = function (asyncUrl, callbackName) {
        var script = document.createElement('script');        
        script.src = asyncUrl + callbackName;
        document.body.appendChild(script);
    };
    
    asyncLoad(asyncUrl, 'googleMapsInitialized');
    
    return mapsDefer.promise;
}]);
