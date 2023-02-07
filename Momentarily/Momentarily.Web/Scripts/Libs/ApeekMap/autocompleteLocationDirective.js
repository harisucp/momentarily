angular.module('Apeek').directive('autocompleteLocation', ['LoadGoogleMapAPI', function (LoadGoogleMapAPI) {
    return {
        restrict: 'EA',
        require: 'ngModel',
        scope: {                       
            latitude: '=',
            longitude: '=',
            searchByMap: '=',
            placeFilters: '=',
            countryFilter: '=',
            useCurrentPosition: '=',
            ngModel: '='            
        },        
        link: function (scope, element, attrs, ngModel) {                                    
            scope.placeFilters = scope.placeFilters || '';                       

            LoadGoogleMapAPI.then(function () {
                initAutocomplete();
                if (scope.useCurrentPosition) setCurrentPosition();                
            });

            function initAutocomplete() {   
debugger             
                var autocomplete = new google.maps.places.Autocomplete(element[0], {
                    types : scope.placeFilters ? scope.placeFilters.split(",") : [],

                    componentRestrictions: scope.countryFilter ? {
 
                        country: scope.countryFilter
                    } : {}
                });

                google.maps.event.addListener(autocomplete, 'place_changed', function (place2) {                    
                    var place = autocomplete.getPlace();
                    if (!place || !place.geometry) {
                        scope.$apply(function () {
                            scope.latitude = 0;
                            scope.longitude = 0;
                            scope.ngModel = null;
                        });
                    } 
                    
                    scope.$apply(function () {                                              
                        scope.latitude = place.geometry.location.lat();
                        scope.longitude = place.geometry.location.lng();                       
                        scope.searchByMap = false;                        
                        scope.ngModel = place.formatted_address;
                    });
                });
                
                scope.$watchGroup(['ngModel', 'latitude', 'longitude'], function (newVal, oldVal) {                    
                    var isValid = angular.isDefined(scope.ngModel) &&
                                  angular.isDefined(scope.latitude) &&
                                  angular.isDefined(scope.longitude);                    
                    ngModel.$setValidity('autocompleter', isValid);
                });
                
            }

            function setCurrentPosition() {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        var geocoder = new google.maps.Geocoder;
                        var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude)
                        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                            if (status !== google.maps.GeocoderStatus.OK) {
                                console.warn(status);
                                return;
                            }

                            scope.$apply(function () {
                                scope.latitude = position.coords.latitude;
                                scope.longitude = position.coords.longitude;
                                scope.ngModel = (results[0].formatted_address);
                            });
                        });
                    });
                }
            }
        }
    };        
}]);