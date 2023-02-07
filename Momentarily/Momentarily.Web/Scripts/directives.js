angular
    .module('MomentarilyApp')
    /* upload image directive*/
    .directive('uploadImage', uploadImage)
    /* upload images multiple directive*/
    .directive('multiplyUpload', multiplyUpload)
    /*datepicker*/
    .directive('ngDatepicker', ngDatepicker)
    /*string to number*/
    .directive('stringToNumber', stringToNumber)
    /*when content in dom rendered*/
    .directive('onFinishRender', onFinishRender)
    /*when height of element change*/
    .directive('onSizeChanged', onSizeChanged)
    /*scroll menu with arrows*/
    .directive('scrollArrowMenu', scrollArrowMenu)
    /*Image upload error*/
    .directive('onErrorSrc', onErrorSrc)
    /*when not work click in mobile browser*/
    .directive('nClick', nClick)
    .directive('preventEnterSubmit', function () {
        return function (scope, el, attrs) {
            el.bind('keydown', function (event) {
                if (13 == event.which) {
                    event.preventDefault(); // Doesn't work at all
                    window.stop(); // Works in all browsers but IE...
                    document.execCommand('Stop'); // Works in IE
                    return false; // Don't even know why it's here. Does nothing.
                }
            });
        };
    })
    .directive('applySelect2', applySelect2)
    .directive('fileUpload', fileUpload)






uploadImage.$inject = ['$http', '$timeout'];

function uploadImage($http, $timeout) {

    var directive = {
        restrict: 'EA',
        scope: {
            'thumbFilePath': '@',
            'largeFilePath': '@',
            'url': '@',
            'status': '=',
            'imgObj': '=',
            'userPhoto': '@'
        },
        controller: controller,
        link: link,
        replace: true,
        template:
            '<div class="photo" ng-class="{success: showImage, error: showError || notFound, empty: !showImage || !showError || !notFound}">' +
            '<div class="photo-container" ng-class="{success: showImage, error: showError || notFound}">' +
            '<photo-block ng-show="showImage" ng-click="openLarge()"></photo-block>' +
            '<div class="controls" ng-show="showImage || notFound">' +
            '<div class="delete" ng-click="delete(imgObj.Id)">' +
            '<div class="text-block">{{(userPhoto)?"Delete picture":"Delete photo"}}</div>' +
            '</div>' +
            '</div>' +
            '<div class="upload-input-block">' +
            '<div class="upload-img" ng-show="showUploadInput && !userPhoto">Add photos here<br/> or <span class="underlined">browse</span></div>' +
            '<div class="upload-img" ng-show="userPhoto && showUploadInput">Upload userphoto</div>' +
            '<div class="upload-img" ng-show="userPhoto && !showUploadInput">Change profile picture</div>' +
            '<div class="upload-img-container" ng-show="userPhoto && showUploadInput"></div>' +
            '<input type="file" accept="image/*">' +
            '</div>' +
            '<div class="loading" ng-show="showLoading"><img class="loading-img" src="/Content/Img/rolling.gif" alt=""></div>' +
            '<div class="error-msg" ng-show="showError">Error upload</div>' +
            '<div class="error-msg" ng-show="notFound">Not Found</div>' +
            '</div>' +
            '</div>'
    };

    return directive;

    function controller($scope, $element) {

        /*status list
        -2 - not found
        -1 - error,
        0 - empty,
        1 - progress,
        2 - completed
        */

        var img = new Image();
        $scope.status = 0;

        $scope.$watch('thumbFilePath', function (newVal) {
            if (newVal) {
                $scope.loadThumb(newVal);
            }
        });

        $scope.$watchCollection('imgObj', function (newVal, oldVal) {
            if (newVal && newVal.file && !newVal.Id && (newVal.file != oldVal.file)) {
                if ($scope.isValidFile(newVal.file)) {
                    $scope.upload(newVal.file);
                }
            }

            if (newVal && !newVal.file && newVal.Id && newVal.Url) {
                $scope.loadThumb(newVal.Url);
            }

        });

        $scope.$watch('status', function (newVal) {
            if (newVal === 0) {
                $scope.showImage = false;
                $scope.showUploadInput = true;
                $scope.showLoading = false;
                $scope.showError = false;
                $scope.notFound = false;
            }
            if (newVal === -2) {
                $scope.showImage = false;
                $scope.showUploadInput = true;
                $scope.showLoading = false;
                $scope.showError = false;
                $scope.notFound = true;
            }
            if (newVal === -1) {
                $scope.showImage = false;
                $scope.showUploadInput = true;
                $scope.showLoading = false;
                $scope.showError = true;
                $scope.notFound = false;
            }
            if (newVal === 1) {
                $scope.showImage = false;
                $scope.showUploadInput = false;
                $scope.showLoading = true;
                $scope.showError = false;
                $scope.notFound = false;
            }
            if (newVal === 2) {
                $scope.showImage = true;
                $scope.showUploadInput = false;
                $scope.showLoading = false;
                $scope.showError = false;
                $scope.notFound = false;
            }
        });

        $scope.loadThumb = function (thumbFilePath) {
            if (thumbFilePath) {
                img.onload = function () {
                    showImage(img);
                }
                img.onerror = function () {
                    $scope.hideImage();
                    $scope.$apply(function () {
                        $scope.status = -2;
                    });
                }
                img.src = thumbFilePath;
            }
        };

        function showImage(img) {
            if (img) {
                var element = angular.element($element.find('photo-block')[0]);
                element.html('');
                element.append(img);
                $scope.$apply(function () {
                    $scope.status = 2;
                });
            } else {
                $scope.$apply(function () {
                    $scope.status = -1;
                });
            }

        }

        $scope.hideImage = function () {
            var element = angular.element($element.find('photo-block')[0]);
            element.html('');
        };

        $scope.upload = function (file) {
            var getModelAsFormData = function (data) {
                var dataAsFormData = new FormData();
                angular.forEach(data, function (value, key) {
                    dataAsFormData.append(key, value);
                });
                return dataAsFormData;
            };
            if ($scope.url) {
                $scope.status = 1;
                $http({
                    method: 'POST',
                    url: $scope.url,
                    data: getModelAsFormData([file]),
                    transformRequest: angular.identity,
                    headers: {
                        'Content-Type': undefined
                    }
                })
                    .success(function (data) {
                        if ($scope.imgObj) {
                            $scope.imgObj.Id = data.Id;
                            $scope.imgObj.FileName = data.FileName;
                            $scope.imgObj.UserId = data.UserId;
                        }

                        $scope.loadThumb(data.Url);
                        $scope.thumbFileName = data.Url;
                        $scope.largeFilePath = data.Url;
                        $scope.id = data.Id;

                        $scope.status = 2;
                    })
                    .error(function (data, status) {
                        $scope.status = -1;
                    });
            } else {
                alert("URL not found!")
            }
        };

        $scope.isValidFile = function (file) {

            var size = 20 * 1024 * 1024;
            if (file.size > size || file.size == 0) {
                alert("File too large. File must be less than 20 megabytes");
            } else if (!file.name.match(/\.jpeg$|\.jpg$|\.png$|\.bmp$/gmi)) {
                alert(file.name + " Invalid file type. (Only jpeg, jpg, png and bmp)");
            } else {
                return 1;
            }
        };

        $scope.delete = function (id) {
            if (id === 0) {
                //$scope.deleted({ id: id });
            } else {
                if (confirm("Delete?")) {
                    $scope.status = 1;
                    $http({
                        method: 'DELETE',
                        url: $scope.url + '/' + id
                    })
                        .success(function (data) {
                            //$scope.deleted({ id: id });
                            $scope.status = 0;

                            if ($scope.imgObj) {
                                for (var prop in $scope.imgObj) delete $scope.imgObj[prop];
                            }
                        })
                        .error(function (data, status) {
                            $scope.status = -1;
                        });
                }
            }
        };

        $scope.openLarge = function () {
            if ($scope.largeFilePath) {
                window.open($scope.largeFilePath);
            }
        };
    }


    function link(scope, element) {

        var fileElement = angular.element(element.find("input")[0]);

        fileElement.bind('change', function () {
            scope.hideImage();
            if (fileElement[0].files.length === 0) {
                return null;
            } else {
                if (fileElement[0].files.length === 1) {
                    scope.status = 1;
                    if (scope.isValidFile(fileElement[0].files[0])) {
                        scope.upload(fileElement[0].files[0]);
                    }
                }
            }
        });
    }
}


multiplyUpload.$inject = ['$filter'];

function multiplyUpload($filter) {
    var directive = {
        restrict: 'EA',
        scope: {
            images: "&",
            countUpload: "@",
            valueImages: "=",
            existImages: "="

        },
        replace: true,
        template: '<div class="photos">' +
            '<upload-image ng-repeat="img in images" img-obj="img" url="/api/momentarilygoodimage"></upload-image>' +
            '</div>',
        link: link
    };
    return directive;


    function link(scope, element) {

        if (scope.existImages && scope.existImages.length) {
            var lenghtExistImages = scope.existImages.length;
            scope.images = scope.existImages;

            if (lenghtExistImages < +scope.countUpload) {
                scope.images.push({});
            }
        }
        else {
            scope.images = [];
            scope.images.push({});
        }


        function createValueImages() {
            scope.valueImages = [];
            for (var i = 0; i < scope.images.length; i++) {
                if (scope.images[i].Id) {
                    scope.valueImages.push({});
                    var length = scope.valueImages.length;
                    scope.valueImages[length - 1].Id = scope.images[i].Id;
                    scope.valueImages[length - 1].FileName = scope.images[i].FileName;
                }
            }
            if (scope.valueImages.length < scope.countUpload) {
                if (scope.valueImages.length === scope.images.length) {
                    scope.images.push({});
                } else {
                    scope.images.length = scope.valueImages.length + 1;
                }
            }
        }


        for (var i = 0; i < scope.countUpload; i++) {
            scope.$watchCollection('images[' + i + ']', function (newVal, oldVal) {
                if (newVal !== oldVal) {
                    scope.images = $filter('orderBy')(scope.images, "Id");
                    createValueImages();
                }
            });
        }
    }
}


ngDatepicker.$inject = ['$document', '$filter'];

function ngDatepicker($document, $filter) {
    'use strict';

    var directive = {
        restrict: 'EA',
        require: '?ngModel',
        scope: {
            minDateRange: '=',
            maxDateRange: '=',
            minDate: '=',
            goodShareDates: '=',
            goodBookedDates: '=',
            goodBookedDatesUntil: '=',
            goodAllStartdates: "=",
            goodAllEnddates: "=",
            dateStart: '='
        },
        link: link,
        template: '<div><input type="text" name="{{name}}" ng-focus="showCalendar($event)" ng-value="viewValue" class="ng-datepicker-input form-control {{class}}" readonly placeholder="{{ placeholder }}" required="{{required}}"></div>' +
            '<div class="ng-datepicker-container" ng-show="calendarOpened">' +
            '<div class="ng-datepicker-background"></div>' +
            '<div class="ng-datepicker" style="{{position}}">' +
            '  <div class="controls">' +
            '    <div class="left">' +
            '      <i class="fa fa-backward prev-year-btn" ng-show="yearView" ng-click="prevListYears()"></i>' +
            '      <i class="fa fa-backward prev-year-btn" ng-hide="yearView" ng-click="prevYear()"></i>' +
            '      <i class="fa fa-angle-left prev-month-btn" ng-show="dayView" ng-click="prevMonth()"></i>' +
            '    </div>' +
            '    <span class="date"><span ng-click="setMonthView()" ng-show="dayView" ng-bind="dateMonthValue"></span>&nbsp;<span ng-hide="yearView" ng-click="setYearView()" ng-bind="dateYearValue"></span><span ng-show="yearView" ng-bind="dateListYearValue"></span></span>' +
            '    <div class="right">' +
            '      <i class="fa fa-forward next-year-btn" ng-show="yearView" ng-click="nextListYears()"></i>' +
            '      <i class="fa fa-forward next-year-btn" ng-hide="yearView" ng-click="nextYear()"></i>' +
            '      <i class="fa fa-angle-right next-month-btn" ng-show="dayView" ng-click="nextMonth()"></i>' +
            '    </div>' +
            '  </div>' +
            '  <div class="day-names" ng-show="dayView">' +
            '    <span ng-repeat="dn in dayNames">' +
            '      <span>{{ dn }}</span>' +
            '    </span>' +
            '  </div>' +
            '  <div class="calendar day-view" ng-show="dayView">' +
            '    <span ng-repeat="d in days">' +
            '      <span class="day" ng-click="selectDate($event, d)" ng-class="{disabled: !d.enabled, active: d.active, selected: d.selected}"><span>{{ d.day }}</span></span>' +
            '    </span>' +
            '  </div>' +
            '  <div class="calendar month-view" ng-show="monthView">' +
            '    <span ng-repeat="m in months">' +
            '      <span class="month" ng-click="selectMonth($event, m)" ng-class="{active: m.active}"><span>{{ m.name }}</span></span>' +
            '    </span>' +
            '  </div>' +
            '  <div class="calendar year-view" ng-show="yearView">' +
            '    <span ng-repeat="y in years">' +
            '      <span class="year" ng-click="selectYear($event, y)"  ng-class="{active: y.active}"><span>{{ y.year }}</span></span>' +
            '    </span>' +
            '  </div>' +
            '</div></div>'
    };

    return directive;

    function setScopeValues(scope, attrs) {
        scope.format = attrs.format || 'MM-DD-YYYY';
        scope.viewFormat = attrs.viewFormat || 'MM-DD-YYYY';
        scope.locale = attrs.locale || 'en';
        scope.minDate = scope.minDate || moment(new Date('1900-01-01')).format(scope.format);
        scope.maxDate = scope.maxDate || moment(new Date('2200-12-31')).format(scope.format);
        scope.firstView = attrs.firstView || 'day';
        scope.viewOrder = attrs.viewOrder ? attrs.viewOrder.split('-') : ['day'];
        scope.firstWeekDaySunday = scope.$eval(attrs.firstWeekDaySunday) || false;
        scope.placeholder = attrs.placeholder || '';
        scope.class = attrs.className || '';
        scope.name = attrs.name || '';
        scope.birthday = attrs.birthday || '';
    };

    function link(scope, element, attrs, ngModel) {
        setScopeValues(scope, attrs);

        scope.calendarOpened = false;
        scope.dayView = false;
        scope.monthView = true;
        scope.yearView = false;
        scope.days = [];
        scope.months = [];
        scope.years = [];
        scope.dayNames = [];
        scope.monthNames = [];
        scope.viewValue = null;
        scope.dateMonthValue = null;
        scope.dateYearValue = null;
        scope.currentView = null;

        moment.locale(scope.locale);
        var isBreakInSharerArray = false;
        //var date = moment();//moment(scope.maxDate).subtract(15, 'year');
        if (scope.goodShareDates) {
            // console.log(scope.goodShareDates);
            if (scope.goodBookedDates) {

                for (var j = 0; j < scope.goodBookedDates.length; j++) {
                    for (var i = 0; i < scope.goodShareDates.length; i++) {
                        if (scope.goodShareDates[i].valueOf() === scope.goodBookedDates[j]) {
                           
                            scope.goodShareDates.splice(i, 1);
                        }
                    }
                }
            }
            scope.goodShareDates.sort(function (a, b) {
                return moment(a, "MM-DD-YYYY").unix() - moment(b, "MM-DD-YYYY").unix();
            });

            for (var i = 0; i < scope.goodShareDates.length; i++) {
                scope.goodShareDates[i] = moment(scope.goodShareDates[i], "MM-DD-YYYY");
            }
        }


        scope.$watch('dateStart', function (val) {
            closeCalendar();
        }, true);

        var date;
        if (scope.birthday) {
            date = moment().subtract(15, 'year');
        } else {
            date = moment();
        }

        function generateCalendar(date) {
            if (scope.dayView) generateDayView(date);
            if (scope.monthView) generateMonthView(date);
            if (scope.yearView) generateYearView(date);
        };

        function generateDayView(date) {
            var lastDayOfMonth = date.endOf('month').date(),
                month = date.month(),
                year = date.year(),
                n = 1;

            var firstWeekDay = scope.firstWeekDaySunday === true ? date.set('date', 2).day() : date.set('date', 1).day();
            if (firstWeekDay !== 1) {
                n -= firstWeekDay - 1;
            }
            if (firstWeekDay === 0) {
                n -= firstWeekDay + 7;
            }

            scope.dateMonthValue = date.format('MMMM');
            scope.dateYearValue = date.format('YYYY');
            scope.days = [];
            isBreakInSharerArray = false;

            for (var i = n; i <= lastDayOfMonth; i += 1) {

                if (i > 0) {
                    scope.days.push({ day: i, month: month + 1, year: year, enabled: true, active: isActiveDay(i, month, year), selected: isSelectDay(i, month, year) });
                } else {
                    scope.days.push({ day: null, month: null, year: null, enabled: false, active: false, selected: false });
                }
            }
        };

        function generateMonthView(date) {
            var year = date.year();

            //scope.dateMonthValue = date.format('MMMM');
            scope.dateYearValue = date.format('YYYY');
            scope.months = [];

            for (var i = 0; i < 12; i += 1) {
                scope.months.push({ name: scope.monthNames[i], month: i, year: year, enabled: true, active: isActiveDay(1, i, year) });
            }
        };

        function generateYearView(date) {
            var year = date.year();

            //scope.dateMonthValue = date.format('MMMM');
            //scope.dateYearValue = date.format('YYYY');

            scope.dateListYearValue = (year - 14) + ' - ' + (year + 15);
            scope.years = [];

            for (var i = year - 14; i < year + 16; i += 1) {
                scope.years.push({ year: i, enabled: true, active: isActiveDay(1, 0, i) });
            }
        };

        function generateDayNames() {
            var date = scope.firstWeekDaySunday === true ? moment('2015-06-07') : moment('2015-06-01');
            for (var i = 0; i < 7; i += 1) {
                scope.dayNames.push(date.format('ddd'));
                date.add('1', 'd');
            }
        };

        function generateMonthNames() {
            var date = moment('01-01-2015');
            for (var i = 0; i < 12; i += 1) {
                scope.monthNames.push(date.format('MMMM'));
                date.add('1', 'M');
            }
        };

        function setPositionCalendar(target) {
            var top = target.offsetTop + target.offsetHeight + 2,
                left = target.offsetLeft + target.offsetWidth / 2 - 125;

            scope.position = "top: " + (top) + "; left: " + (left) + "px;";
        };

        function isActiveDay(day, month, year) {
            if (scope.birthday) {
                console.log("birthday : " + scope.birthday);
                return true;
            }
            var minDate = moment(scope.minDate, scope.format),
                maxDate = moment(scope.maxDate, scope.format),
                selectDate = moment({ year: year, month: month, day: day });

            function isDateinArray(day, arrayOfDays) {
                var isDateIn = false;
                for (var d = 0; d < arrayOfDays.length; d++) {
                    if (day.isSame(arrayOfDays[d], 'day')) {
                        isDateIn = true;
                        break;
                    }
                }
                return isDateIn;
            }

            //datepicker "from"
            if (scope.name === "dateStart" && selectDate.isBetween(minDate, maxDate, 'day', '[]')) {
                 for(var b = 0; b < scope.goodAllStartdates.length; b++) {
                     var index = scope.goodShareDates.indexOf(scope.goodShareDates.find(function (x) { return x._i === scope.goodAllStartdates[b]; } ));
                if(index>=1){
                   scope.goodShareDates.splice(index,1);
                    }
               };

                   for (var a = 0; a < scope.goodAllEnddates.length; a++) {
                 var modifieddate=moment(scope.goodAllEnddates[a], "MM-DD-YYYY");
                 var index = scope.goodShareDates.indexOf(scope.goodShareDates.find(function (x) { return x._i == scope.goodAllEnddates[a]; } ));
                if(index<1){
                     scope.goodShareDates.push(modifieddate);
                }
                 };

                for (var j = 0; j < scope.goodShareDates.length; j++) {
                    if (selectDate.isSame(scope.goodShareDates[j], 'day')) {
                        return true;
                    };
                }
            }
            //datepicker "to if from is choosen"
            if (scope.name === "dateEnd") {
                      for(var b = 0; b < scope.goodAllStartdates.length; b++) {
                     var modifieddate=moment(scope.goodAllStartdates[b], "MM-DD-YYYY");
                     var index = scope.goodShareDates.indexOf(scope.goodShareDates.find(function (x) { return x._i == scope.goodAllStartdates[b]; }))
                    if(index<1){
                     scope.goodShareDates.push(modifieddate);
                        }
                         }

                   for (var a = 0; a < scope.goodAllEnddates.length; a++) {
                       var index=scope.goodShareDates.indexOf(scope.goodShareDates.find(function (x) { return x._i==scope.goodAllEnddates[a]; }))
                if(index>=1){
                   scope.goodShareDates.splice(index,1);
                    }
                 };



                if (scope.dateStart) {
                     var newdate =  new Date(scope.dateStart);
                        newdate.setDate(newdate.getDate()+1);
                        var month=newdate.getMonth()+1;
                        if(month<10)
                        {
                        month='0'+month;
                        }
                    var minimumdate =month+'/'+newdate.getDate()+'/'+newdate.getFullYear();
                    minDate = moment(minimumdate, scope.format);
                
                    if (selectDate.isSameOrAfter(minDate, 'day')) {

                        if (!isBreakInSharerArray) {
                            if (isDateinArray(selectDate, scope.goodShareDates)) {
                                return true;
                            } else {
                                isBreakInSharerArray = true;
                            }
                        }
                    }
                }
                else {
            

                    for (var j = 0; j < scope.goodShareDates.length; j++) {
                        if ((selectDate.isSame(minDate) || selectDate.isBetween(minDate, maxDate)) && selectDate.isSame(scope.goodShareDates[j])) {
                            return true;
                        }
                    }

            
                }
            }

            return false;
        }

        function isSelectDay(day, month, year) {
            if (scope.viewValue) {
                var dateValue = moment(scope.viewValue, scope.viewFormat);
                var selectDate = moment((month + 1) + '-' + day + '-' + year, "MM-DD-YYYY"),
                    selectedDate = moment((dateValue.month() + 1) + '-' + dateValue.date() + '-' + dateValue.year(), "MM-DD-YYYY");

                if (selectDate.isSame(selectedDate)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        function setView(view) {
            // 0 - dayView
            // 1 - monthView
            // 2 - yearView

            if (view === 'day') {
                scope.dayView = true;
                scope.monthView = false;
                scope.yearView = false;
            }
            if (view === 'month') {
                scope.dayView = false;
                scope.monthView = true;
                scope.yearView = false;
            }
            if (view === 'year') {
                scope.dayView = false;
                scope.monthView = false;
                scope.yearView = true;
            }
            scope.currentView = view;
        }

        function setViewFromAttr() {

            setView(scope.viewOrder[0]);
            //if (scope.firstView === 'day') {
            //    setView(0);
            //}
            //if (scope.firstView === 'month') {
            //    setView(1);
            //}
            //if (scope.firstView === 'year') {
            //    setView(2);
            //}
        }

        function closeCalendar() {
            $document.off('click');
            scope.calendarOpened = false;
        };

        element.on("click", function (e) {
            if (e.target.className === "ng-datepicker-background") {
                clickOutside(e);
            }
        });

        function checkRange(selectedDate) {
            setValueModel(selectedDate);
            /*
                        if (!scope.maxDateRange && !scope.minDateRange) {
                            
                        } else {
                            if (scope.maxDateRange) {
                                
                            }
            
                            if (scope.minDateRange) {
                                
                            }
                        }*/
        }

        function setValueModel(selectedDate) {
            ngModel.$setViewValue(selectedDate.format(scope.format));
            scope.viewValue = selectedDate.format(scope.viewFormat);
            closeCalendar();
        }

        generateDayNames();
        generateMonthNames();
        setViewFromAttr();


        scope.showCalendar = function ($event) {
            //$(".ng-datepicker-container").addClass("ng-hide");
            ;
            setPositionCalendar($event.target);
            scope.calendarOpened = true;
            $document.on('click', clickOutside);
            generateCalendar(date);
        };

        scope.prevListYears = function () {
            date.subtract(30, 'Y');
            generateCalendar(date);
        };

        scope.prevYear = function () {
            date.subtract(1, 'Y');
            generateCalendar(date);
        };

        scope.prevMonth = function () {
            date.subtract(1, 'M');
            generateCalendar(date);
        };

        scope.nextMonth = function () {
            date.add(1, 'M');
            generateCalendar(date);
        };

        scope.nextListYears = function () {
            date.add(30, 'Y');
            generateCalendar(date);
        };

        scope.nextYear = function () {
            date.add(1, 'Y');
            generateCalendar(date);
        };

        var setNextView = function () {
            if (scope.currentView) {
                var index = scope.viewOrder.indexOf(scope.currentView);
                if (index > -1) {
                    //scope.currentView = scope.viewOrder[++index];
                    setView(scope.viewOrder[++index]);
                }
            }
        };

        scope.setMonthView = function () {
            //setView('month');
            setNextView();
            generateCalendar(date);
        };

        scope.setYearView = function () {
            //setView('year');
            setNextView();
            generateCalendar(date);
        };

        scope.selectDate = function (event, dateObj) {
            if (dateObj.active) {
                date.date(dateObj.day);
                if (isLastView()) {
                    setDate(event, dateObj);
                    return;
                }
                setNextView();
                //setView(0);
                generateCalendar(date);
                //event.preventDefault();
                //var selectedDate = moment(dateObj.day + '.' + dateObj.month + '.' + dateObj.year, 'DD.MM.YYYY');
                //checkRange(selectedDate);
            }
        };

        scope.selectMonth = function (event, dateObj) {
            if (dateObj.active) {
                date.month(dateObj.month);
                if (isLastView()) {
                    setDate(event, dateObj);
                    return;
                }
                setNextView();
                //setView(0);
                generateCalendar(date);
            }
        };

        scope.selectYear = function (event, dateObj) {
            if (dateObj.active) {
                date.year(dateObj.year);
                if (isLastView()) {
                    setDate(event, dateObj);
                    return;
                }
                setNextView();
                //setView(1);
                generateCalendar(date);
            }
        };

        function isLastView() {
            return scope.viewOrder.indexOf(scope.currentView) === (scope.viewOrder.length - 1);
        }

        function setDate(event, dateObj) {

            setView(scope.viewOrder[0]);

            event.preventDefault();
            var selectedDate = moment(date.date() + '.' + (date.month() + 1) + '.' + date.year(), 'DD.MM.YYYY');
            checkRange(selectedDate);

        }

        function clickOutside(e) {
            if (!scope.calendarOpened) return;

            var i = 0,
                element;

            if (!e.target) return;
            if (e.target.className === 'ng-datepicker-background') {
                closeCalendar();
                scope.$apply();
                return;
            }

            for (element = e.target; element; element = element.parentNode) {
                var id = element.id;
                var classNames = element.className;

                if (id !== undefined) {
                    for (i = 0; i < classList.length; i += 1) {
                        if (id.indexOf(classList[i]) > -1 || classNames.indexOf(classList[i]) > -1) {
                            return;
                        }
                    }
                }
            }

            closeCalendar();
            scope.$apply();
        }

        // if clicked outside of calendar
        var classList = ['ng-datepicker', 'ng-datepicker-input'];
        if (attrs.id !== undefined) classList.push(attrs.id);

        ngModel.$render = function () {
            var newValue = ngModel.$viewValue;
            if (newValue !== undefined) {
                scope.viewValue = moment(newValue).format(attrs.viewFormat);
                scope.dateValue = newValue;
            }
        };

    }
}


function stringToNumber() {

    var directive = {
        require: 'ngModel',
        link: link
    };

    return directive;

    function link(scope, element, attrs, ngModel) {
        ngModel.$parsers.push(function (value) {
            return '' + value;
        });
        ngModel.$formatters.push(function (value) {
            return parseFloat(value, 10);
        });
    }
}


onFinishRender.$inject = ['$timeout', '$parse'];

function onFinishRender($timeout, $parse) {
    var directive = {
        restrict: 'A',
        link: link
    }

    return directive;

    function link(scope, element, attr) {
        if (scope.$last === true) {
            $timeout(function () {
                scope.$emit('ngRepeatFinished');
                if (!!attr.onFinishRender) {
                    $parse(attr.onFinishRender)(scope);
                }
            });
        }
    }
}

function onSizeChanged() {
    return {
        restrict: 'A',
        scope: {
            onSizeChanged: '&'
        },
        link: function (scope, $element, attr) {
            var element = $element[0];
            var startHeight = element.offsetHeight;


            setInterval(function () {
                if (startHeight != element.offsetHeight) {
                    onWindowResize(element.offsetHeight, startHeight);
                    startHeight = element.offsetHeight;
                }
            }, 200)

            function onWindowResize(height, startHeight) {
                var expression = scope.onSizeChanged();
                expression(height, startHeight);
            };
        }
    }
}

function scrollArrowMenu() {
    var directive = {
        restrict: 'A',
        link: link,
        replace: true,
        scope: {},
        template: '<div class="controlArrows" ng-class="{active: containerWidth<scrollWidth}"><i class="fa fa-arrow-left" ng-class="{active: scrollLeft != 0}" ng-click="scrollingLeft()"></i><i class="fa fa-arrow-right" ng-class="{active: !((scrollWidth - containerWidth) === scrollLeft)}" ng-click="scrollingRight()"></i></div>'
    };

    return directive;

    function link(scope, element, attrs) {
        var container = document.querySelector('.' + attrs.scrollArrowMenu),
            list = container.querySelector('.menu-list'),
            timer;

        scope.scrollLeft = 0;
        scope.containerWidth = container.style.width;
        scope.scrollWidth = list.scrollWidth;

        function whenScroll() {
            if (timer !== null) {
                clearTimeout(timer);
            }
            timer = setTimeout(function () {
                scope.scrollLeft = container.scrollLeft;
                scope.containerWidth = container.clientWidth;
                scope.scrollWidth = list.scrollWidth;
                scope.$apply();

            }, 100);
        }

        function scrollTo(element, px, duration) {
            if (duration <= 0) return;
            var perTick = px / duration * 10;

            setTimeout(function () {
                element.scrollLeft = element.scrollLeft + perTick;
                if (element.scrollLeft == px) return;
                scrollTo(element, px, duration - 10);
            }, 10);
        }

        function scrollLeft() {
            //container.scrollLeft -= 25;
            scrollTo(container, -40, 100);
        }

        function scrollRight() {
            scrollTo(container, 40, 100);
        }

        scope.scrollingLeft = scrollLeft;
        scope.scrollingRight = scrollRight;

        container.addEventListener("scroll", whenScroll, false);
        window.addEventListener("resize", whenScroll, false);
    }
}

function onErrorSrc() {
    var directive = {
        restrict: 'A',
        link: link
    };

    return directive;

    function link(scope, element, attrs) {
        element.bind('error', function () {
            if (attrs.src != attrs.onErrorSrc) {
                attrs.$set('src', attrs.onErrorSrc);
            }
        });
    }
}

function nClick() {
    var directive = {
        restrict: 'A',
        link: link
    };
    return directive;

    function link(scope, element, attrs) {
        element.bind('click', function (event) {
            scope.$apply(attrs['nClick']);
        });
    }
}


function applySelect2() {

    var directive = {
        require: 'ngModel',
        link: link
    };

    return directive;

    function link(scope, element, attrs, ngModel) {
        var select2 = $(element).select2().trigger('change');

        select2.on('change', function () {
            var val = $(this).val();
            console.log('on change event', val);

            scope.$apply(function () {
                //will cause the ng-model to be updated.
                ngModel.$setViewValue(val);
            });
        });

        //ngModel.$render = function () {
        //    ;
        //    //if this is called, the model was changed outside of select, and we need to set the value
        //    //not sure what the select2 api is, but something like:
        //    element.value = ngModel.$viewValue;

        //}
    }


}

function fileUpload() {
    return {
        scope: true,
        link: function (scope, el, attrs) {
            el.bind('change', function (event) {
                var files = event.target.files;
                //iterate files since 'multiple' may be specified on the element
                if (files.length == 0) {
                    
                    scope.$emit("fileSelected", { file: null, field: event.target.name });
                } else {
                    for (var i = 0; i < files.length; i++) {
                        //emit event upward
                        scope.$emit("fileSelected", { file: files[i], field: event.target.name });
                    }
                }
            });
        }
    };
}