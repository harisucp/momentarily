angular
    .module('MomentarilyApp')
    .service('ListingService', ListingService);

ListingService.$inject = ['$http'];

function ListingService($http) {
    return {
        DeleteItem: function (id) {
            var apiUrl = "/api/MomentarilyItemArchive/";
            return $http.delete(apiUrl + id);
        },
        CancelRequest: function (id) {
            var urlRequest = "/Listing/CancelBooking/" + id;
            return $http.get(urlRequest);
        }
    };
}