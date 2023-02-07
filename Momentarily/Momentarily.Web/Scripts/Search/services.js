angular
    .module('MomentarilyApp')
    .service('SearchService', SearchService);

SearchService.$inject = ['$http'];

function SearchService($http) {
    return {
        PostItems: function (data) {
            var apiUrl = "/api/MomentarilyItemMap";
            return $http.post(apiUrl, data);
        }
        //,
        //couponDetail: function (CouponCode, CustomerCost) {
        //      var apiUrl = "/Listing/GetCouponDetail";
        //    return $http.get(apiUrl, data);
        //}


    };
}



