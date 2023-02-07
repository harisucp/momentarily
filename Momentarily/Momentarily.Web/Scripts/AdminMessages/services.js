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
}