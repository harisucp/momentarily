'format amd';(function(){'use strict';function isUndefinedOrNull(val){return angular.isUndefined(val)||val===null;}
function requireMoment(){try{return require('moment');}catch(e){throw new Error('Please install moment via npm. Please reference to: https://github.com/urish/angular-moment');}}
function angularMoment(angular,moment){if(typeof moment==='undefined'){if(typeof require==='function'){moment=requireMoment();}else{throw new Error('Moment cannot be found by angular-moment! Please reference to: https://github.com/urish/angular-moment');}}
angular.module('angularMoment',[]).constant('angularMomentConfig',{preprocess:null,timezone:null,format:null,statefulFilters:true}).constant('moment',moment).constant('amTimeAgoConfig',{withoutSuffix:false,serverTime:null,titleFormat:null,fullDateThreshold:null,fullDateFormat:null,fullDateThresholdUnit:'day'}).directive('amTimeAgo',['$window','moment','amMoment','amTimeAgoConfig',function($window,moment,amMoment,amTimeAgoConfig){return function(scope,element,attr){var activeTimeout=null;var currentValue;var withoutSuffix=amTimeAgoConfig.withoutSuffix;var titleFormat=amTimeAgoConfig.titleFormat;var fullDateThreshold=amTimeAgoConfig.fullDateThreshold;var fullDateFormat=amTimeAgoConfig.fullDateFormat;var fullDateThresholdUnit=amTimeAgoConfig.fullDateThresholdUnit;var localDate=new Date().getTime();var modelName=attr.amTimeAgo;var currentFrom;var isTimeElement=('TIME'===element[0].nodeName.toUpperCase());var setTitleTime=!element.attr('title');function getNow(){var now;if(currentFrom){now=currentFrom;}else if(amTimeAgoConfig.serverTime){var localNow=new Date().getTime();var nowMillis=localNow-localDate+amTimeAgoConfig.serverTime;now=moment(nowMillis);}
else{now=moment();}
return now;}
function cancelTimer(){if(activeTimeout){$window.clearTimeout(activeTimeout);activeTimeout=null;}}
function updateTime(momentInstance){var timeAgo=getNow().diff(momentInstance,fullDateThresholdUnit);var showFullDate=fullDateThreshold&&timeAgo>=fullDateThreshold;if(showFullDate){element.text(momentInstance.format(fullDateFormat));}else{element.text(momentInstance.from(getNow(),withoutSuffix));}
if(titleFormat&&setTitleTime){element.attr('title',momentInstance.format(titleFormat));}
if(!showFullDate){var howOld=Math.abs(getNow().diff(momentInstance,'minute'));var secondsUntilUpdate=3600;if(howOld<1){secondsUntilUpdate=1;}else if(howOld<60){secondsUntilUpdate=30;}else if(howOld<180){secondsUntilUpdate=300;}
activeTimeout=$window.setTimeout(function(){updateTime(momentInstance);},secondsUntilUpdate*1000);}}
function updateDateTimeAttr(value){if(isTimeElement){element.attr('datetime',value);}}
function updateMoment(){cancelTimer();if(currentValue){var momentValue=amMoment.preprocessDate(currentValue);updateTime(momentValue);updateDateTimeAttr(momentValue.toISOString());}}
scope.$watch(modelName,function(value){if(isUndefinedOrNull(value)||(value==='')){cancelTimer();if(currentValue){element.text('');updateDateTimeAttr('');currentValue=null;}
return;}
currentValue=value;updateMoment();});if(angular.isDefined(attr.amFrom)){scope.$watch(attr.amFrom,function(value){if(isUndefinedOrNull(value)||(value==='')){currentFrom=null;}else{currentFrom=moment(value);}
updateMoment();});}
if(angular.isDefined(attr.amWithoutSuffix)){scope.$watch(attr.amWithoutSuffix,function(value){if(typeof value==='boolean'){withoutSuffix=value;updateMoment();}else{withoutSuffix=amTimeAgoConfig.withoutSuffix;}});}
attr.$observe('amFullDateThreshold',function(newValue){fullDateThreshold=newValue;updateMoment();});attr.$observe('amFullDateFormat',function(newValue){fullDateFormat=newValue;updateMoment();});attr.$observe('amFullDateThresholdUnit',function(newValue){fullDateThresholdUnit=newValue;updateMoment();});scope.$on('$destroy',function(){cancelTimer();});scope.$on('amMoment:localeChanged',function(){updateMoment();});};}]).service('amMoment',['moment','$rootScope','$log','angularMomentConfig',function(moment,$rootScope,$log,angularMomentConfig){var defaultTimezone=null;this.changeLocale=function(locale,customization){var result=moment.locale(locale,customization);if(angular.isDefined(locale)){$rootScope.$broadcast('amMoment:localeChanged');}
return result;};this.changeTimezone=function(timezone){if(moment.tz&&moment.tz.setDefault){moment.tz.setDefault(timezone);$rootScope.$broadcast('amMoment:timezoneChanged');}else{$log.warn('angular-moment: changeTimezone() works only with moment-timezone.js v0.3.0 or greater.');}
angularMomentConfig.timezone=timezone;defaultTimezone=timezone;};this.preprocessDate=function(value){if(defaultTimezone!==angularMomentConfig.timezone){this.changeTimezone(angularMomentConfig.timezone);}
if(angularMomentConfig.preprocess){return angularMomentConfig.preprocess(value);}
if(!isNaN(parseFloat(value))&&isFinite(value)){return moment(parseInt(value,10));}
return moment(value);};}]).filter('amParse',['moment',function(moment){return function(value,format){return moment(value,format);};}]).filter('amFromUnix',['moment',function(moment){return function(value){return moment.unix(value);};}]).filter('amUtc',['moment',function(moment){return function(value){return moment.utc(value);};}]).filter('amUtcOffset',['amMoment',function(amMoment){function amUtcOffset(value,offset){return amMoment.preprocessDate(value).utcOffset(offset);}
return amUtcOffset;}]).filter('amLocal',['moment',function(moment){return function(value){return moment.isMoment(value)?value.local():null;};}]).filter('amTimezone',['amMoment','angularMomentConfig','$log',function(amMoment,angularMomentConfig,$log){function amTimezone(value,timezone){var aMoment=amMoment.preprocessDate(value);if(!timezone){return aMoment;}
if(aMoment.tz){return aMoment.tz(timezone);}else{$log.warn('angular-moment: named timezone specified but moment.tz() is undefined. Did you forget to include moment-timezone.js ?');return aMoment;}}
return amTimezone;}]).filter('amCalendar',['moment','amMoment','angularMomentConfig',function(moment,amMoment,angularMomentConfig){function amCalendarFilter(value,referenceTime,formats){if(isUndefinedOrNull(value)){return'';}
var date=amMoment.preprocessDate(value);return date.isValid()?date.calendar(referenceTime,formats):'';}
amCalendarFilter.$stateful=angularMomentConfig.statefulFilters;return amCalendarFilter;}]).filter('amDifference',['moment','amMoment','angularMomentConfig',function(moment,amMoment,angularMomentConfig){function amDifferenceFilter(value,otherValue,unit,usePrecision){if(isUndefinedOrNull(value)){return'';}
var date=amMoment.preprocessDate(value);var date2=!isUndefinedOrNull(otherValue)?amMoment.preprocessDate(otherValue):moment();if(!date.isValid()||!date2.isValid()){return'';}
return date.diff(date2,unit,usePrecision);}
amDifferenceFilter.$stateful=angularMomentConfig.statefulFilters;return amDifferenceFilter;}]).filter('amDateFormat',['moment','amMoment','angularMomentConfig',function(moment,amMoment,angularMomentConfig){function amDateFormatFilter(value,format){if(isUndefinedOrNull(value)){return'';}
var date=amMoment.preprocessDate(value);if(!date.isValid()){return'';}
return date.format(format);}
amDateFormatFilter.$stateful=angularMomentConfig.statefulFilters;return amDateFormatFilter;}]).filter('amDurationFormat',['moment','angularMomentConfig',function(moment,angularMomentConfig){function amDurationFormatFilter(value,format,suffix){if(isUndefinedOrNull(value)){return'';}
return moment.duration(value,format).humanize(suffix);}
amDurationFormatFilter.$stateful=angularMomentConfig.statefulFilters;return amDurationFormatFilter;}]).filter('amTimeAgo',['moment','amMoment','angularMomentConfig',function(moment,amMoment,angularMomentConfig){function amTimeAgoFilter(value,suffix,from){var date,dateFrom;if(isUndefinedOrNull(value)){return'';}
value=amMoment.preprocessDate(value);date=moment(value);if(!date.isValid()){return'';}
dateFrom=moment(from);if(!isUndefinedOrNull(from)&&dateFrom.isValid()){return date.from(dateFrom,suffix);}
return date.fromNow(suffix);}
amTimeAgoFilter.$stateful=angularMomentConfig.statefulFilters;return amTimeAgoFilter;}]).filter('amSubtract',['moment','angularMomentConfig',function(moment,angularMomentConfig){function amSubtractFilter(value,amount,type){if(isUndefinedOrNull(value)){return'';}
return moment(value).subtract(parseInt(amount,10),type);}
amSubtractFilter.$stateful=angularMomentConfig.statefulFilters;return amSubtractFilter;}]).filter('amAdd',['moment','angularMomentConfig',function(moment,angularMomentConfig){function amAddFilter(value,amount,type){if(isUndefinedOrNull(value)){return'';}
return moment(value).add(parseInt(amount,10),type);}
amAddFilter.$stateful=angularMomentConfig.statefulFilters;return amAddFilter;}]).filter('amStartOf',['moment','angularMomentConfig',function(moment,angularMomentConfig){function amStartOfFilter(value,type){if(isUndefinedOrNull(value)){return'';}
return moment(value).startOf(type);}
amStartOfFilter.$stateful=angularMomentConfig.statefulFilters;return amStartOfFilter;}]).filter('amEndOf',['moment','angularMomentConfig',function(moment,angularMomentConfig){function amEndOfFilter(value,type){if(isUndefinedOrNull(value)){return'';}
return moment(value).endOf(type);}
amEndOfFilter.$stateful=angularMomentConfig.statefulFilters;return amEndOfFilter;}]);return'angularMoment';}
if(typeof define==='function'&&define.amd){define(['angular','moment'],angularMoment);}else if(typeof module!=='undefined'&&module&&module.exports&&(typeof require==='function')){module.exports=angularMoment(require('angular'),require('moment'));}else{angularMoment(angular,(typeof global!=='undefined'?global:window).moment);}})();