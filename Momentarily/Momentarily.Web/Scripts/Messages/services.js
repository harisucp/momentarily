angular
    .module('MomentarilyApp')
    .service('MessageService', MessageService);

MessageService.$inject = ['$http'];

function MessageService($http) {
    return {
        PostMessage: function (data) {
         
            var apiUrl = "/api/UserMessage/";
            return $http.post(apiUrl, data);
        },
        ReadMessage: function(data) {
            var apiUrl = "/api/UserMessage/";
            return $http.put(apiUrl, data);
        }
    }

    //uploadFileToUrl = function (file, uploadUrl) {
    //    debugger
    //    return $http({
    //        method: 'POST',
    //        url: uploadUrl,
    //        headers: { 'Content-Type': undefined },
    //        transformRequest: function () {
    //            var formData = new FormData();
    //            if (file) {
    //                formData.append("myFile", file);
    //            }
    //            return formData;
    //        }
    //    })
    //}

}