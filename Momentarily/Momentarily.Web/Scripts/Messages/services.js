angular
    .module('MomentarilyApp')
    .service('MessageService', MessageService);

MessageService.$inject = ['$http'];

function MessageService($http) {
    return {
        PostMessage: function (data) {
            var apiUrl = "/api/UserMessage/Post";

            // Set the Content-Type header to 'application/json'
            var config = {
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            debugger;
            data.IsSystem = false;
            // Use $http.post with the data and config
            return $http.post(apiUrl, data, config).then(handleSuccess, handleError);
        },
        ReadMessage: function (data) {
            var apiUrl = "/api/UserMessage/";

            // Use $http.put with the data
            return $http.put(apiUrl, data).then(handleSuccess, handleError);
        }
    };

    // Function to handle successful HTTP requests
    function handleSuccess(response) {
        return response.data;
    }

    // Function to handle errors in HTTP requests
    function handleError(error) {
        // You can log the error or perform additional error handling here
        console.error('HTTP request failed:', error);
        throw error;
    }
}