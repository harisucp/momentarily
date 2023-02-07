angular
    .module('MomentarilyApp')
    .service('ReviewService', ReviewService);

ReviewService.$inject = ['$http'];

function ReviewService($http) {
    return {
        GetSeekersReview: function (userId, page) {
            var data = {userId:userId, page:page}
            var apiUrl = "/api/MomentarilySeekerReview/";
            return $http.get(apiUrl, {params:data});
        },
        GetSharersReview: function (userId, page) {
            var data = { userId: userId, page: page }
            var apiUrl = "/api/MomentarilySharerReview/";
            return $http.get(apiUrl, { params: data });
        }
    }
}