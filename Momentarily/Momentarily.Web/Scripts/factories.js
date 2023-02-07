angular
    .module('MomentarilyApp')
    //.factory('mapsInit', mapsInitFactory)
    .factory('clrDateTime', clrDateTime);

function clrDateTime() {
    var convertClrDateTimeToJavaScriptDate = function (clrDateTime) {
        return new Date(parseInt(clrDateTime.substr(6)));
    };

    return {
        convertToDate: convertClrDateTimeToJavaScriptDate
    };
}