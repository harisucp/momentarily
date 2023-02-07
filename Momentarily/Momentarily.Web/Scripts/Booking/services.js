angular
    .module('MomentarilyApp')
    .service('BookingService', BookingService);

BookingService.$inject = ['$http'];

function BookingService($http) {
    return {
        CancelRequest: function (id) {
            var apiUrlRequest = "/Booking/CancelRequest/" + id;
            return $http.get(apiUrlRequest);
        }
    }
}