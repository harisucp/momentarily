angular.module('Apeek').directive('apeekMap', ['$timeout', '$parse', '$interpolate', '$compile', 'LoadGoogleMapAPI', function ($timeout, $parse, $interpolate, $compile, LoadGoogleMapAPI) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            items: "=",
            coordChange: "&",
            changeOnMoveMap: "=",
            searchLocation: "=",
            showRoutes: "=",
            offset: "=",
            distanceAndRouteFromCurrentLocation: '='
        },
        template: '<div id="map"></div>',
        link: function (scope, element, attrs) {
            var self = this;
            self.map = null;
            var openedInfoWindow = null;
            self.geocoder = null;
            self.markers = [];
            self.markerBounds = null;
            self.infowindow = null;

            self.initMap = function () {
                self.map = new google.maps.Map(element[0], {
                    zoom: 17,
                    //scrollwheel: false,
                    // center: myLatLng,
                    gestureHandling: 'none',
                    zoomControl: false,
                    //zoomControl: false,
                    //zoomControlOptions: {
                    //    style: google.maps.ZoomControlStyle.LARGE,
                    //    position: google.maps.ControlPosition.LEFT_TOP
                    //},
                    disableDefaultUI: true
                    //mapTypeId: google.maps.MapTypeId.ROADMAP
                });

                self.geocoder = new google.maps.Geocoder();
                self.distanceMatrixService = new google.maps.DistanceMatrixService();
                self.directionsService = new google.maps.DirectionsService();
                self.directionsDisplay = new google.maps.DirectionsRenderer({ draggable: true });
                self.directionsDisplay.setMap(self.map);
                self.directionsDisplay.setOptions({ suppressMarkers: true });
                self.directionsDisplay.preserveViewport = true;

                google.maps.event.addListener(self.map, "click", function () {
                    if (openedInfoWindow != null) {
                        openedInfoWindow.close();
                    }
                });

                self.markerBounds = new google.maps.LatLngBounds();

                self.map.addListener('dragend', self.coordinateChanged);
                self.map.addListener('zoom_changed', self.coordinateChanged);
            }

            self.bindWatchers = function () {
                scope.$watch('items', function (items) {
                    if (angular.isArray(items)) {
                        self.drawItems(items);
                        scope.showRoutes && self.drawDirections(items);
                        self.calculateItemsDistances(items);
                    }
                    self.addCurrentLocationMarker();
                });
                scope.$watch('changeOnMoveMap', self.coordinateChanged);
            }

            self.getLocationByAddress = function (address, callback) {
                if (!callback) return;

                self.geocoder.geocode({ 'address': address }, function (results, status) {
                    if (status === google.maps.GeocoderStatus.OK) {
                        callback(null, {
                            address: address,
                            location: results ? results[0] : null
                        });
                    } else {
                        callback(status);
                    }
                });
            }

            self.addCurrentLocationMarker = function () {
                if (!scope.searchLocation) {
                    return;
                }

                self.getLocationByAddress(scope.searchLocation, function (err, result) {
                    if (err) {
                        alert('Error while trying to fetch the exact location');
                        //alert('Geocode was not successful for the following reason: ' + err);
                        return;
                    }

                    var item = {
                        Latitude: result.location.geometry.location.lat(),
                        Longitude: result.location.geometry.location.lng()
                    };
                    markerStyle = {
                        pinDimension: "big",
                        size: new google.maps.Size(34, 34),
                        point1: new google.maps.Point(0, 0),
                        point2: new google.maps.Point(10, 10)
                    };



                    self.addMarkerOnMap(item, -1, markerStyle);
                });
            }

            self.calculateItemsDistances = function (items) {
                $timeout(function () {
                    for (var i = 0; i < items.length; i++) {
                        (function (item) {
                            self.distanceMatrixService.getDistanceMatrix({
                                origins: scope.distanceAndRouteFromCurrentLocation ? [scope.searchLocation] : [item.Location],
                                destinations: scope.distanceAndRouteFromCurrentLocation ? [item.Location] : [scope.searchLocation],
                                travelMode: google.maps.TravelMode.DRIVING,
                                unitSystem: google.maps.UnitSystem.IMPERIAL,
                                avoidHighways: false,
                                avoidTolls: false
                            }, function (response, status) {
                                scope.$apply(function () {
                                    item.distanceToCurrentLocation = $parse('rows[0].elements[0].distance')(response);
                                });
                            });
                        })(items[i]);
                    }
                }, 0);
            }

            self.drawItems = function (items) {
                self.deleteMarkers();

                for (var i = 0; i < items.length; i++) {
                    self.addMarkerOnMap(items[i], i,
                        {
                            pinDimension: "little",
                            size: new google.maps.Size(12, 12),
                            point1: new google.maps.Point(0, 0),
                            point2: new google.maps.Point(0, 0)
                        });
                }

                if (scope.searchLocation && !scope.changeOnMoveMap) {
                    self.map.fitBounds(self.markerBounds);
                    self.geocodeAddress(scope.searchLocation);
                }
            }

            self.drawDirections = function (items) {
                for (var i = 0; i < items.length; i++) {
                    self.directionsService.route({
                        origin: scope.distanceAndRouteFromCurrentLocation ? scope.searchLocation : items[i].Location,
                        destination: scope.distanceAndRouteFromCurrentLocation ? items[i].Location : scope.searchLocation,
                        travelMode: google.maps.TravelMode["DRIVING"]
                    }, function (response, status) {
                        if (status == google.maps.DirectionsStatus.OK) {
                            self.directionsDisplay.setDirections(response);
                        }
                    });
                }
            }

            self.geocodeAddress = function (address) {
                self.getLocationByAddress(address, function (err, result) {
                    if (err) {
                        alert('Error while trying to fetch the exact location');
                        //alert('Geocode was not successful for the following reason: ' + err);
                        return;
                    }
                    self.map.setCenter(result.location.geometry.location);
                    self.map.fitBounds(result.location.geometry.viewport);
                });
            }

            self.addMarkerOnMap = function (item, index, markerStyle) {

                var pinImage = new google.maps.MarkerImage(
                    "../../../Content/Img/marker-" + markerStyle.pinDimension + ".png",
                    markerStyle.size,
                    markerStyle.point1,
                    markerStyle.point2
                );

                var marker = new google.maps.Marker({
                    position: { lat: item.Latitude, lng: item.Longitude },
                    map: self.map,
                    //icon: pinImage,
                    title: item.Name,
                    zIndex: 1001000
                });

                if (markerStyle.pinDimension === "little") {

                    marker.addListener('click', openInfoBox);

                    function openInfoBox() {
                        var infoBoxTemplate = '' +
                            '<div>' +
                            '<div class="list-items">' +
                            '<a target="_blank" href="/Search/Item/{{GoodId}}" class="item">' +
                            '<div class="info-box-item">' +
                            '<div class="img-item">' +
                            '<div class="img-container">' +
                            '<img src="{{GoodImageUrl}}" alt="">' +
                            '</div>' +
                            '</div>' +
                            '<div class="description">' +
                            '<ng-rate-it ng-model="Rank" read-only="true"></ng-rate-it>' +
                            '<div class="title">{{Name}}</div>' +
                            '<div class="distance">{{distanceToCurrentLocation.text}}</div>' +
                            '<div class="price">${{Price}}<span> per day</span></div>' +
                            '</div>' +
                            '</div>' +
                            '</a>' +
                            '</div>' +
                            '</div>';

                        var compiledInfoBoxTemplate = $compile($interpolate(infoBoxTemplate)(item))(scope)[0];
                        scope.$digest();
                        if (scope.offset === true) {
                            self.infowindow = new google.maps.InfoWindow({
                                content: compiledInfoBoxTemplate.innerHTML,
                                pixelOffset: new google.maps.Size(-1, 218)
                            });
                            open();
                            var anchor = angular.element(document.getElementsByClassName('gm-style-iw'));
                            var children = anchor.parent().children();
                            var child;
                            if (children.length > 0) {
                                child = angular.element(children[0]);
                            }
                            var secondLevelChildren = child.children();
                            if (secondLevelChildren.length > 0) {
                                secondLevelChild1 = angular.element(secondLevelChildren[0]).css("margin-top", "-160px").css("transform", "rotateX(180deg)");
                                secondLevelChild3 = angular.element(secondLevelChildren[2]).css("margin-top", "-160px").css("transform", "rotateX(180deg)");
                            }
                        } else {
                            self.infowindow = new google.maps.InfoWindow({
                                content: compiledInfoBoxTemplate.innerHTML
                            });
                            open();
                        }

                        function open() {
                            if (openedInfoWindow != null) {
                                openedInfoWindow.close();
                            }
                            openedInfoWindow = self.infowindow;
                            self.infowindow.open(self.map, marker);
                        }



                    };
                }

                var elem = document.getElementById('item' + index);

                if (elem) {
                    elem.addEventListener("mouseover", toggleBounce);
                    elem.addEventListener("mouseleave", notToggleBounce);
                }

                function toggleBounce(e) {
                    marker.setAnimation(google.maps.Animation.BOUNCE);
                }

                function notToggleBounce(e) {
                    marker.setAnimation(null);
                }

                self.markers.push(marker);

                self.markerBounds.extend(marker.position);
            }

            self.deleteMarkers = function () {
                for (var i = 0; i < self.markers.length; i++) {
                    self.markers[i].setMap(null);
                }
                self.markers = [];
            }

            self.coordinateChanged = function (e) {
                if (scope.changeOnMoveMap) {
                    var northEast = self.map.getBounds().getNorthEast();
                    var southWest = self.map.getBounds().getSouthWest();

                    if (northEast && southWest) {
                        var neLat = northEast.lat(),
                            neLng = northEast.lng(),
                            swLat = southWest.lat(),
                            swLng = southWest.lng();

                        scope.coordChange()(neLat, neLng, swLat, swLng, e);
                    }
                }
            }

            LoadGoogleMapAPI.then(function () {
                self.initMap();
                self.bindWatchers();

            });

        }
    }
}]);
