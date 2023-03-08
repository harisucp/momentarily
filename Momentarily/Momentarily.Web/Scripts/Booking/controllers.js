angular
    .module('MomentarilyApp')
    .controller('BookingController', BookingController)
    .controller('BookingRequestController', BookingRequestController);

BookingController.$inject = ['clrDateTime', 'Booking'];

function BookingController(clrDateTime, Booking) {

    var vm = this;

    vm.showBookingsList = showBookingsList;
    vm.convertToDate = clrDateTime.convertToDate;

    setValues();

    function setValues() {
        vm.items = Booking.ViewModel;
    }

    function showBookingsList() {
        return vm.items.length !== 0;
    }
}

BookingRequestController.$inject = ['clrDateTime', 'BookingService', 'Request'];

function BookingRequestController(clrDateTime, BookingService, Request) {    

    var vm = this;

    vm.convertToDate = clrDateTime.convertToDate;
    vm.cancelRequest = cancelRequest;
    vm.isCanceled = isCanceled;
    vm.isApproved = isApproved;
    vm.isPaid = isPaid; 
    vm.isDeclined = isDeclined; 
    vm.isRelease = isRelease; 
    vm.isReceived = isReceived;
    vm.isReturned = isReturned;
    vm.isReturnedConfirm = isReturnedConfirm;
    vm.isLate = isLate;
    vm.isDamaged = isDamaged;
    vm.isLateAndDamaged = isLateAndDamaged;
    vm.isNotresponded = isNotresponded;
    vm.isDispute = isDispute;
    vm.showContactInfo = showContactInfo;
    setValues();

    function setValues() {
        vm.item = Request.ViewModel;        
    }

    function cancelRequest($event) {
        
        var userConfirms = confirm("Are You sure You want to cancel Your booking?");
        if (userConfirms) {
            //var requestId = vm.item.Id;
            //BookingService.CancelRequest(requestId);
        } else {
            $event.preventDefault();
        }
    }

    function isCanceled() {        return (vm.item.StatusId === 4 || vm.item.StatusId === 28);    }

    function isApproved() {
        return vm.item.StatusId === 2;
    }

    function isPaid() {
        return vm.item.StatusId === 5;
    }

    function isDeclined() {
        return vm.item.StatusId === 3;
    }
    function isDispute() {
        return vm.item.StatusId === 10;
    }

    function isRelease() {
        return vm.item.StatusId === 6;
    }
    function isReceived() {
        return vm.item.StatusId === 13;
    }
   function isNotresponded() {
        return vm.item.StatusId === 12;
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
        return isPaid() || isRelease() || isDispute();
    }
}