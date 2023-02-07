angular.module('MomentarilyApp').controller('ListingEditController', ['$scope', '$window', 'ListingService', 'CreateItem', 'createAction', 'editAction', function ($scope, $window, ListingService, CreateItem, createAction, editAction) {
    var self = this;
    
    var key = CreateItem.momentarilyItemDepositKey;

    var data = CreateItem.categories;
    self.Categorynames = [];
    for (var i = 0; i < data.length; i++) {
        self.Categorynames.push({ id: data[i].Key, text: data[i].Value });
    }
    self.categories = self.Categorynames;

    self.types = CreateItem.types;
    self.submitForm = submitForm;
    self.deleteItem = deleteItem;
    self.itemExists = itemExists;
    self.getselectedid = getselectedid;
    self.ChangeMinimumPeriod = ChangeMinimumPeriod;
    
    setValues();
    function setValues() {
        
        if (self.itemExists) {
            ;
            self.item = CreateItem.item;
            self.item.CategoryId = self.item.CategoryId ? self.item.CategoryId : null;
            self.item.CategorList = self.item.CategorList != '' ? (typeof self.item.CategorList === "object" ? self.item.CategorList: self.item.CategorList.split(',')) : '';
            self.item.selectedItems = [];
            self.item.Price = self.item.Price == '0' && 'number' ? '' : self.item.Price;
            self.item.PricePerWeek = self.item.PricePerWeek == '0' && 'number' ? '' : self.item.PricePerWeek;
            //self.item.Price = typeof self.item.Price == 'number'? self.item.Price.toFixed(2) : self.item.Price;
            //self.item.PricePerWeek = typeof self.item.PricePerWeek == 'number' ? self.item.PricePerWeek.toFixed(2) : self.item.PricePerWeek;
            self.item.PricePerMonth = typeof self.item.PricePerMonth == 'number' ? self.item.PricePerMonth.toFixed(2) : self.item.PricePerMonth;
            self.item.GoodPropertyValues[key].Value = self.item.GoodPropertyValues[key].Value != "" && self.item.GoodPropertyValues[key].Value != null ? parseFloat(self.item.GoodPropertyValues[key].Value).toFixed(2) : self.item.GoodPropertyValues[key].Value;
            self.action = editAction;
            self.item.IsMinimumperiodError = false;
            self.item.MinimumPeriodMessage = '';
            self.images = null;
            self.images = CreateItem.item.Images;
            self.arrayValueImages = null;
            self.arrayValueImages = CreateItem.item.Images;
        } else {
            self.item = {
                Id: 0,
                GoodLocation: {
                    Latitude: 0,
                    Longitude: 0
                },
                PickUpLocation: {
                    Latitude: 0,
                    Longitude: 0
                },
                RentPeriodDay: true,
                CategoryId: null,
                //CategorList: self.item.CategorList
              
            
            };
            self.item.CategorList = self.item.CategorList && self.item.CategorList != '' ? self.item.CategorList.split(',') : '';
            self.item.selectedItems = [];
            //set default type = Equipment
            //dropdown type is disabled
            self.item.GoodPropertyValues = {
                MomentarilyItem_Type: { PropertyValueDefinitionId: 1 }
            };
            self.action = createAction;
        }
        self.item.GoodShareDates = self.item.GoodShareDates || [];
        self.showhideErrorforMaxPrice = false;


    self.getSelectedItems = function (item) {
        return item.selected;
    };

    }    

    function ChangeMinimumPeriod() {
        if (!self.item.RentPeriodDay && !self.item.RentPeriodWeek && self.item.MinimumRentPeriod > 0) {
            self.item.IsMinimumperiodError = true;
            self.item.MinimumPeriodMessage = 'Please select rent period';
            self.item.MinimumRentPeriod = 0;
        }
        else if ((self.item.RentPeriodDay || self.item.RentPeriodWeek) && self.item.MinimumRentPeriod > 21) {
            self.item.IsMinimumperiodError = true;
            self.item.MinimumPeriodMessage = 'Maximum 21 days allowed';
            
        }
        else if (!self.item.RentPeriodDay && self.item.RentPeriodWeek && self.item.MinimumRentPeriod < 7) {
            self.item.IsMinimumperiodError = true;
            self.item.MinimumPeriodMessage = 'Minimum 7 days allowed';
           
        }
        else {
            self.item.IsMinimumperiodError = false;
            self.item.MinimumPeriodMessage = '';
            //self.item.MinimumRentPeriod = null;
        }


    }
    function submitForm($event, form) {
        ListingEditController
        if (form.$invalid) {
            $event.preventDefault();
            form.$submitted = true;
        }
    }

    function deleteItem() {
        var userConfirms = confirm("Are You sure You want to delete this item?");
        if (userConfirms) {            
            self.deleting = true;
            ListingService.DeleteItem(CreateItem.item.Id).then(function (response) {
                $window.location = "/Listing/";
            }, function (response) {
                self.deleting = false;
                alert("We can not delete this item. Error...");
            });
        }
    }

    function itemExists() {
       
        return CreateItem.item.Id > 0;
    }

    function getselectedid(prasad) {
        ;
        self.item.CategorList = [];
        for (var i = 0; i < prasad.length; i++) {
            self.item.CategorList.push(prasad[i].id);
        }
        console.log("selected category",self.item.CategorList);
    }
}]);


angular.module('MomentarilyApp').directive('allowOnlyNumbers', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }
                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keycode === 32) {
                    event.preventdefault();
                }
            });
        }
    };
});

//angular.module('MomentarilyApp').directive("preventTypingGreater", function () {
//    return {
//        link: function (scope, element, attributes) {
//            var oldVal = null;
//            element.on("keydown keyup", function (e) {
//                if (Number(element.val()) > Number(attributes.max) &&
//                    e.keyCode != 46 // delete
//                    &&
//                    e.keyCode != 8 // backspace
//                ) {
//                    e.preventDefault();
//                    element.val(oldVal);
//                } else {
//                    oldVal = Number(element.val());
//                }
//            });
//        }
//    };
//});



//angular.module('MomentarilyApp').directive('allowOnlyNumbers', function () {
//    return {
//        restrict: 'A',
//        link: function (scope, elm, attrs, ctrl) {
//            elm.on('keydown', function (event) {
//                var value = this.value;
//                value = value.replace(/[^0-9.]/g, '')
//                this.value = value;
//                if (event.which == 64 || event.which == 16) {
//                    // to allow numbers  
//                    return false;
//                } else if (event.which >= 48 && event.which <= 57) {
//                    // to allow numbers  
//                    return true;
//                } else if (event.which >= 96 && event.which <= 105) {
//                    // to allow numpad number  
//                    return true;
//                } else if ([8, 9, 13, 27, 37, 38, 39, 40, 190].indexOf(event.which) > -1) {
//                    // to allow backspace, enter, escape, arrows, decimal
//                    return true;
//                } else {
//                    event.preventDefault();
//                    // to stop others  
//                    //alert("Sorry Only Numbers Allowed");  
//                    return false;
//                }
//            });
//        }
//    }
//});  