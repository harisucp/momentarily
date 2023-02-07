angular
    .module('MomentarilyApp')
    .filter('newlines', newlines)
    .filter('encodeBase64', encodeBase64)
    .filter('addSpanTag', addSpanTag);

function newlines() {
    return function (text) {
        return text.replace(/(\r)?\n/g, '<br />');
        //return text.replace(/\n/g, '<br/>');
    }
}


function encodeBase64() {
    return function (text) {
        return btoa(text);
    };
}



function addSpanTag($sce) {
    
    return function (text) {
        var textArr = text.split('/');
        var htmlArr = textArr.map(function (txt) {
            return '<span>' + txt + '</span>';
        });
        return $sce.trustAsHtml(htmlArr.join('/ '));
    };
}