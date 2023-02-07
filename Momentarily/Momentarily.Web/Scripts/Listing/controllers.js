angular
    .module('MomentarilyApp')    
    .controller('BookingListController', BookingListController)
    .controller('BookingRequestController', BookingRequestController)
    .controller('BookingDeclineRequestController', BookingDeclineRequestController)
    .controller('BookingDisputeReqeustController', BookingDisputeReqeustController)
    .controller('BookingReviewRequestController', BookingReviewRequestController)
    .controller('DatePickerController', DatePickerController);


BookingListController.$inject = ['clrDateTime', 'BookingList'];
function BookingListController(clrDateTime, BookingList) {

    var vm = this;

    vm.convertToDate = clrDateTime.convertToDate;
    vm.isEmpty = isEmpty;

    setValues();

    function setValues() {
        vm.requests = BookingList.ViewModel.GoodRequests;
        vm.itemName = BookingList.ViewModel.GoodName;
    }

    function isEmpty() {
        return vm.requests.length === 0;
    }
}

BookingRequestController.$inject = ['clrDateTime', 'BookingRequest', 'ListingService'];

function BookingRequestController(clrDateTime, BookingRequest, ListingService) {
    var vm = this;

    vm.convertToDate = clrDateTime.convertToDate;

    vm.isDeclined = isDeclined;
    vm.isApproved = isApproved;
    vm.isSeekerPaid = isSeekerPaid;
    vm.isPending = isPending;
    vm.isCanceled = isCanceled;
    vm.cancelRequest = cancelRequest;
    vm.isDispute = isDispute;
    vm.isRefunded = isRefunded; 
    vm.isRelease = isRelease; 
    vm.isReceived = isReceived; 
    vm.isReturned = isReturned;
    vm.isReturnedConfirm = isReturnedConfirm;
    vm.isLate = isLate;
    vm.isDamaged = isDamaged;
    vm.isLateAndDamaged = isLateAndDamaged;

    vm.showContactInfo = showContactInfo;

    vm.item = BookingRequest.ViewModel;

    function cancelRequest($event) {
        
        var userConfirms = confirm("Are You sure You want to cancel Your booking?");
        if (userConfirms) {
            ListingService.cancelRequest();
        } else {
            $event.preventDefault();
        }
    }

    function isDeclined() {
        return vm.item.StatusId === 3;
    }

    function isApproved() {
        return vm.item.StatusId === 2;
    }

    function isPending() {
        return vm.item.StatusId === 1;
    }

    function isCanceled() {        return (vm.item.StatusId === 26 || vm.item.StatusId === 27);    }

    function isDispute() {
        return vm.item.StatusId === 10;
    }

    function isRefunded() {
        return vm.item.StatusId === 11;
    }

    function isSeekerPaid() {
        return vm.item.StatusId === 5;
    }
    function isRelease() {
        return vm.item.StatusId === 6;
    }
    function isReceived() {
        return vm.item.StatusId === 13;
    }
    function isReturned() {
        return vm.item.StatusId === 14;
    }
    function isReturnedConfirm() {
        return vm.item.StatusId === 15;
    }
    function isLate() {
        return vm.item.StatusId === 16;
    }
    function isDamaged() {
        return vm.item.StatusId === 18;
    }
     function isLateAndDamaged() {
        return vm.item.StatusId === 20;
    }
    function showContactInfo() {
        return isSeekerPaid() || isRelease() || isDispute();
    }
}

BookingDeclineRequestController.$inject = ['Request'];

function BookingDeclineRequestController(Request) {

    var vm = this;

    setValues();


    function setValues() {
        vm.viewModel = Request.ViewModel;
    }
}

BookingDisputeReqeustController.$inject = ['Request'];
function BookingDisputeReqeustController(Request) {

    var vm = this;

    setValues();

    function setValues() {
        vm.viewModel = Request.ViewModel;
    }
}

BookingReviewRequestController.$inject = ['Request'];
function BookingReviewRequestController(Request) {

    var vm = this;
    vm.viewModel = Request.ViewModel;
    vm.submitForm = submitForm;

    vm.Review = {
        Message: '',
        Rank: '',
    }
    setValues();

    function setValues() {
        vm.viewModel = Request.ViewModel;
        console.log("sss", vm.viewModel);
    }


    function submitForm($event, form) {
        if (form.$invalid) {
            $event.preventDefault();
            form.$submitted = true;
        }
    }
}


DatePickerController.$inject = ['$scope', '$controller'];

function DatePickerController($scope, $controller) {
    
    $controller('ListingEditController', { $scope: $scope });
    
    var vm = this;
    vm.activeDate = null;
    vm.selectedDates = $scope.$parent.listingEditController.item.GoodShareDates;
    vm.day = $scope.$parent.listingEditController.item.RentPeriodDay;
    vm.week = $scope.$parent.listingEditController.item.RentPeriodWeek;
    vm.type = (!vm.day) && vm.week ? 'range':'individual';
    console.log(vm.selectedDates);
    vm.removeFromSelected = removeFromSelected;
    vm.removeFromSelectedRenge = removeFromSelectedRenge;

    function removeFromSelected(dt) {
        vm.selectedDates.splice(vm.selectedDates.indexOf(dt), 1);
    }

    function removeFromSelectedRenge() {
        vm.activeDate = null;
        vm.selectedDates = [];
        $scope.$parent.listingEditController.item.GoodShareDates = [];
    }
}
