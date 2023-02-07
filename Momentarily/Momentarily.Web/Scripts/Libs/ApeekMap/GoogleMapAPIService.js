angular.module('Apeek').service('GoogleMapAPIService', ['$parse', 'LoadGoogleMapAPI', function ($parse, LoadGoogleMapAPI) {
    var self = this;

    self.calculateDistance = function (prms, callback) {
        LoadGoogleMapAPI.then(function () {
            var distanceMatrixService = new google.maps.DistanceMatrixService();

            distanceMatrixService.getDistanceMatrix({
                origins: [prms.from],
                destinations: [prms.to],
                travelMode: google.maps.TravelMode.DRIVING,
                unitSystem: google.maps.UnitSystem.IMPERIAL,
                avoidHighways: false,
                avoidTolls: false
            }, function (response, status) {
                if (status == google.maps.DistanceMatrixStatus.OK) {
                    var responseElement = $parse('rows[0].elements[0]')(response);                    
                    if (callback) {
                        if (responseElement.status == "OK") {
                            callback(null, responseElement.distance);
                        } else {
                            callback(responseElement.status);
                        }
                    }
                } else {
                    if (callback) callback("Error: " + status);
                }
            });
        });        
    }    
}]);
