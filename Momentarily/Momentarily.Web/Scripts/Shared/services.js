angular
    .module('MomentarilyApp')
    .service('UserDataService', UserDataService);

UserDataService.$inject = ['$http'];

function UserDataService($http) {
    return {
        GetUserData: function() {
            var apiUrl = "/api/UserData/";
            return $http.get(apiUrl);
        }
    }
}