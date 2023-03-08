angular
    .module('MomentarilyApp')
    .config(config)
    .controller('SearchController', SearchController)
    .controller('SearchItemController', SearchItemController);

config.$inject = ['$locationProvider'];
function config($locationProvider) {
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false,
        rewriteLinks: false
    });
}

SearchController.$inject = ['$scope', '$log', '$compile', '$location', '$interpolate', 'SearchService', 'Filter', 'Items', 'Categories'];

function SearchController($scope, $log, $compile, $location, $interpolate, SearchService, Filter, Items, Categories) {
    var vm = this;
    
    vm.filter = Filter;
   
    vm.items = Items.Goods;
    vm.itemsCount = Items.Count;
    vm.format = 'MM.DD.YYYY';
    vm.viewFormat = 'MM.DD.YYYY';
    vm.minDateStart = moment().format(vm.format);
    vm.minDateEnd = moment().add(1, 'd').format(vm.format);
    vm.loadingItems = false;
    vm.passItems = passItems;
    vm.mapCoordChanged = mapCoordChanged;
    vm.Categories = Categories;
    vm.Radiusoptions = [1, 5, 10, 25, 50];
   
    vm.sortlist = [{

        "sortId": 1,
        "name": "Price Low to High"
    }, {
        "sortId": 2,
            "name": "Price High to low"
        },
        //{
        //"sortId": 3,
        //    "name": "Least Rented"
        //},
        {
            "sortId": 4,
            "name": "Most Rented"
        },
        //{
        //    "sortId": 5,
        //    "name": "Least Rated"
        //},
         {
            "sortId": 6,
            "name": "Highest Rated"
        }
    ];

    vm.rentperiod = [{

        "rentId": 1,
        "name": "Day"
    }, {
        "rentId": 2,
        "name": "Week"
    }, {
        "rentId": 4,
        "name": "Any"
    }
    ];

    bindWathers();
    searchItems();
    vm.findlocation = findMe;

    function findMe() {

        setLoading('loadingItems', true);

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition);

        } else {

            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    }
    function showPosition(position) {
        //x.innerHTML = "Latitude: " + position.coords.latitude +
        //    "<br>Longitude: " + position.coords.longitude;
        vm.filter.latitude = position.coords.latitude;
        vm.filter.longitude = position.coords.longitude;
        vm.filter.Latitude = position.coords.latitude;
        vm.filter.Longitude = position.coords.longitude;

        //vm.filter.NeLatitude = position.coords.latitude;
        //vm.filter.NeLongitude = position.coords.longitude;

        //vm.filter.SwLatitude = position.coords.latitude;
        //vm.filter.SwLongitude = position.coords.longitude;

        GetAddress(position.coords.latitude, position.coords.longitude);

    }

    function GetAddress(latitude, longitude) {
        

        var lat = parseFloat(latitude);
        var lng = parseFloat(longitude);
        var latlng = new google.maps.LatLng(lat, lng);
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    
                    vm.filter.location = results[0].formatted_address;
                    vm.filter.Location = results[0].formatted_address;
                    $("#clicklocation").val('');
                    $("#clicklocation").val(results[0].formatted_address);
                    $location.search('Location', results[0].formatted_address);
                    searchItems();

                    //window.location.reload();

                    //$location.reload();
                    //getCurrentLocation();
                    //alert("Location: " + results[0].formatted_address);
                }
            }
        });

    }

    function searchItems() {
        setLoading('loadingItems', true);
        SearchService.PostItems(vm.filter).then(setItems, showError);
    }





    function passItems() {
        vm.itemsMap = vm.items;
    }

    function bindWathers() {
        var watchExpressions = [];

        angular.forEach(vm.filter, function (value, key) {
            if ("Location" !== key) {
                this.push('search.filter.' + key);
            }
        }, watchExpressions);

        $scope.$watchGroup(watchExpressions, getItems);

        function getItems(current, original) {
            if (!angular.equals(current, original)) {
                searchItems();
            }
        }
    }

    function mapCoordChanged(neLat, neLng, swLat, swLng, digest) {
        vm.filter.NeLatitude = neLat;
        vm.filter.NeLongitude = neLng;
        vm.filter.SwLatitude = swLat;
        vm.filter.SwLongitude = swLng;

        if (typeof (digest) === "undefined") {
            $scope.$digest();
        }
    }

    function setItems(res) {
        setLoading('loadingItems', false);
        document.getElementById("listing-container").scrollTop = 0;
        window.scrollTo(0, 0)

        vm.itemsCount = res.data.Count;
        vm.items = res.data.Goods;

        if (vm.items.length === 0) {
            passItems();
        }

        $location.search(vm.filter);
    }

    function setLoading(nameObject, value) {
        if (nameObject && typeof (nameObject) === "string" && typeof (value) === "boolean") {
            vm[nameObject] = value;
        }
    }

    function showError() {
        setLoading('loadingItems', true);
    }

}


SearchItemController.$inject = ['$scope', '$http', 'Item', '$sce', '$filter', '$location', '$timeout', 'ReviewService', 'GoogleMapAPIService'];
function SearchItemController($scope, $http, Item, $sce, $filter, $location, $timeout, ReviewService, GoogleMapAPIService) {
    var vm = this;
    
    vm.item = Item;
    console.log("=======items======", Item);
    vm.format = 'MM/DD/YYYY';
    vm.viewFormat = 'MM-DD-YYYY';
    vm.minDateStart = moment().format(vm.format);
    vm.minDateEnd = moment().format(vm.format);
    vm.openInNewTab = OpenInNewTab;
    vm.submitForm = submitForm;
    vm.changeMargin = changeMargin;
    vm.trustedHtml = trustedHtml; 
    vm.isCalculate = false;
    vm.Price = 0;
    vm.Days = 0;
    vm.TotalDaysPrice = 0;
    vm.ServiceFee = 0;
    vm.DiliveryCost = 0;
    vm.Total = 0;
    vm.UntilEnable = "none";
    vm.PriceFieldsEnable = false;
    vm.IsMinimumRental = false;
    vm.loadMoreSeekerReview = loadMoreSeekerReview;
    vm.showMoreSeekerReview = false;
    vm.isSeekerLoading = false;
    vm.ApplyForDelivery = false;
    vm.errorImageUrl = "/Content/Img/error-bg.png";
    vm.activeImageUrl = vm.item.ListingImages.length === 0 ? angular.copy(vm.errorImageUrl) : vm.item.ListingImages[0].LargeImageUrl;
    vm.mapsrc = "https://maps.google.com/maps?q=" + vm.item.Latitude + "," + vm.item.Longitude + "&hl=en&z=15&amp&output=embed";

    vm.SetUntil = SetUntil;
    vm.SetIsMinimumRental = SetIsMinimumRental;
    vm.SetUntils = SetUntils;

    vm.chkselct = false;
    vm.GetCouponsDetail = getCouponDetail;
    vm.CouponCode = "";
    vm.CustomerCost = "";
    vm.couponErrorMessageStatus = false;
    vm.couponSucessMessageStatus = false;
    vm.couponAmountGreaterTotalAmt = false;
    vm.BlankCouponText = false;
    //vm.price.CouponDiscountPrice = 0;
    vm.StartTime = Item.StartTime;    vm.EndTime = Item.EndTime;
    vm.PopUpDates = PopUpDates;
    $scope.searchItem.shippingLocation = {
        Name: $location.search().SearchLocation,
        Latitude: $location.search().SearchLatitude,
        Longitude: $location.search().SearchLongitude
    };

    bindDateWatcher();
    bindDateEndWatcher();
    setValues();
  
    function getCouponDetail() {
        vm.couponErrorMessageStatus = false;
        vm.ValidCouponCode = false;
        vm.couponAmountGreaterTotalAmt = false;
        var codeValue = document.getElementById("couponcodeId").value;
        if (codeValue === '' || codeValue === undefined) {
            vm.BlankCouponText = true;
            var ele = document.getElementById("hh");
            ele.className = "ng-show";
            ele.style.color = "red";
            vm.couponSucessMessageStatus = false;
            return false;
        }
        else {
            var elem = document.getElementById("hh");
            elem.className = "ng-hide";
        }

        console.log("data", vm.item);
        var couponCode = codeValue;
        var customerCost = vm.price.CustomerCost;
        var goodId = vm.item.GoodId;
        var startDate = vm.dateStart;
        var enddDate = vm.dateEnd;
        var shippingDistance = null;
        if ($scope.searchItem.ApplyForDelivery) {
            shippingDistance = vm.ShippingDistance;
        }
        else {
            shippingDistance = 0;
        }
        var ApplyForDelivery = vm.ApplyForDelivery;
        

        vm.calculateAmountErrorMessage = "";
        $http({

            method: 'GET',
            params: {
                CouponCode: couponCode,
                CustomerCost: customerCost,
                GoodId: goodId,
                StartDate: startDate,
                EndDate: enddDate,
                ShippingDistance: shippingDistance,
                ApplyForDelivery: ApplyForDelivery
            },
            url: '/Listing/GetCouponDetail'
        }).then(

            function successCallback(response) {
                if (response.data.Data.message === "" && response.data.Data.CouponDiscount !== -1) {
                   

                    vm.price.CouponDiscount = response.data.Data.CouponDiscount;
                    vm.CouponDiscount = response.data.Data.CouponDiscount;
                    vm.price.CustomerCost = response.data.Data.newCustomerCost;
                    vm.couponSucessMessageStatus = true;
                    vm.ValidCouponCode = false;
                    vm.couponAmountGreaterTotalAmt = false;
                }
                else if (response.data.Data.message !== "" && response.data.Data.CouponDiscount === -2) {

                    vm.couponAmountGreaterTotalAmt = true;
                }
                else {
                    vm.couponErrorMessageStatus = true;
                    vm.ValidCouponCode = true;
                    vm.couponAmountGreaterTotalAmt = false;
                    vm.couponErrorMessage = response.data.Data.message;
                    console.log(response.data.Data.message);
                }
            }
        );
    }

    function SetUntil() {
        
        var _startDate = vm.dateStart;
        if (_startDate === 'undefined' || _startDate === '') {
            vm.UntilEnable = "none";
        }
        else {
            vm.UntilEnable = "block";
        }

    } 

    function SetUntils() {
        
        var newdate =  new Date(vm.dateStart);
        newdate.setDate(newdate.getDate()+1);
            var month=newdate.getMonth()+1;
            if(month<10)
            {
            month='0'+month;
            }
      var day=newdate.getDate();
      if(day<10)
            {
            day='0'+day;
            }

        vm.dateEnd =month+'/'+day+'/'+newdate.getFullYear();
        PopUpDates();
        SetIsMinimumRental();
    }

    function SetIsMinimumRental() {
        var start = new Date(vm.dateStart);
        var end = new Date(vm.dateEnd);
        var difference = (end - start) / (1000 * 3600 * 24);
        if (difference >= vm.item.MinimumRentPeriod) {
            vm.IsMinimumRental = true;
        }
        else {
            vm.IsMinimumRental = false;
        }

        if (difference >= 1) {
            vm.PriceFieldsEnable = true;
        }
        else {
            vm.PriceFieldsEnable = false;
        }
    }

    function PopUpDates() {
        
        var start = vm.dateStart;
        var end = vm.dateEnd;
        if (start === end) {
            var datemodel = document.getElementById('datemodel');
            var trlist = datemodel.getElementsByTagName("tr");
            var startdateday = new Date(start).getDay();
            vm.PriceFieldsEnable = false;
      
        }
        else {
            var datemodels = document.getElementById('datemodel');
            var trlists = datemodels.getElementsByTagName("tr");
            vm.PriceFieldsEnable =true;
        }
        var input = document.getElementsByClassName('ng-datepicker-input');
    }

    function setValues() {
        vm.showMoreSeekerReview = vm.item.SeekersReviews.Items.length < vm.item.SeekersReviews.Pagination.TotalItems;
    }

    function getPrice(goodId, startDate, endDate, shippingDistance, applyForDelivery) {

        if (!goodId || !startDate || !endDate) return;

        vm.calculateAmountErrorMessage = "";
        $http({
            method: 'GET',
            params: {
                GoodId: goodId,
                StartDate: startDate,
                EndDate: endDate,
                ShippingDistance: shippingDistance,
                ApplyForDelivery: applyForDelivery
            },
            url: '/Payment/CalculatePrice'
        }).then(
            function successCallback(response) {
                if (!response.data.errorMessage) {
                    vm.price = response.data;
                } else {
                    vm.calculateAmountErrorMessage = response.data.errorMessage;
                    console.log(response.data.errorMessage);
                }
            }
        );
    }

    function calculateAmount() {

        vm.minDateEnd = vm.dateStart || moment().format(vm.format);
        if (vm.dateEnd) {
            vm.dateEnd = (moment(vm.dateStart) > moment(vm.dateEnd)) ? vm.dateStart : vm.dateEnd;
        }

        vm.price = undefined;

        if ($scope.searchItem.ApplyForDelivery) {
            GoogleMapAPIService.calculateDistance(
                {
                    from: $scope.searchItem.item.Location,
                    to: $scope.searchItem.shippingLocation.Name,
                },
                function (err, result) {
                    $timeout(function () {
                        if (!err) {
                            vm.ShippingDistance = parseFloat(result.text);
                            getPrice(vm.item.GoodId, vm.dateStart, vm.dateEnd, vm.ShippingDistance, vm.ApplyForDelivery);
                        } else {
                            vm.calculateAmountErrorMessage = "Delivery is not available for this destination";
                        }
                    }, 0);
                }
            );
        } else {
            getPrice(vm.item.GoodId, vm.dateStart, vm.dateEnd, 0, vm.ApplyForDelivery);
        }
    }

    function bindDateWatcher() {
        var watchExpressions = ['searchItem.dateEnd', 'searchItem.dateStart', 'searchItem.ApplyForDelivery', 'searchItem.shippingLocation.Name'];
        $scope.$watchGroup(watchExpressions, calculateAmount);
    }

    function bindDateEndWatcher() {
        $scope.$watch('searchItem.dateStart', clearDateEnd);
    }

    function clearDateEnd() {
        if (vm.dateStart) {

        }
    }

    function OpenInNewTab(url) {
        var win = window.open(url, '_blank');
        win.focus();
    }

    function submitForm($event, form) {
       
            if (form.$invalid) {
                $event.preventDefault();
                form.$submitted = true;
            }
            else {
                document.getElementById("chkselctApplyCoupon").checked = false;
                document.getElementById("couponcodeId").value = '';

            }
       
    }

    function changeMargin(height) {
        var startHeight = 219;
        if (window.innerWidth > 992) {
            if (startHeight < height) {
                document.getElementById("user-info-container").style.marginTop = height - startHeight + "px";
            }
        }
    }

    function trustedHtml(html) {
        return $sce.trustAsHtml($filter('newlines')(html));
    }

    function loadMoreSeekerReview() {
        vm.isSeekerLoading = true;
        ReviewService.GetSeekersReview(vm.item.User.Id, vm.item.SeekersReviews.Pagination.CurrentPage + 1).then(function (result) {
            result.data.Items.forEach(function (item) {
                vm.item.SeekersReviews.Items.push(item);
            });
            vm.item.SeekersReviews.Pagination = result.data.Pagination;
            vm.showMoreSeekerReview = vm.item.SeekersReviews.Items.length < vm.item.SeekersReviews.Pagination.TotalItems;
            //angular.extend(vm.profileData.SeekersReviews.Items, result.data.Items);

        }).then(function () {
            vm.isSeekerLoading = false;
        });
    }

}
