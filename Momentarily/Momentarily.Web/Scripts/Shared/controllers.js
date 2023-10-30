angular
    .module('MomentarilyApp')
    .controller('LayoutController', LayoutController);

LayoutController.$inject = ['$scope', '$interval', 'UserDataService'];

function LayoutController($scope, $interval, UserDataService) {
    var vm = this;
    vm.UserData = {};
    vm.hasUnreadMessages = hasUnreadMessages;
    vm.UserNotification = UserNotification;
    if (window.innerWidth < 768) {
        vm.isMobile = true;
    }
    
    vm.isLoading = false;
    setValues();



    $scope.$on('reload-userdata', function () {
        loadUserData();
    });

    function hasUnreadMessages() {
        if (vm.UserData && vm.UserData.UnreadMessagesCount) {
            return vm.UserData.UnreadMessagesCount > 0;
        }
        return false;
    }

    function UserNotification() {
        if (vm.UserData && vm.UserData.UserNotification) {
            return vm.UserData.UserNotification > 0;
        }
        return false;
    }

    function setValues() {
        loadUserData();
    }
    
    //function loadUserData() {
    //    if (!vm.isLoading) {
    //        vm.isLoading = true;
    //        UserDataService.GetUserData().then(function(result) {
    //            vm.UserData = result.data;
    //            vm.isLoading = false;
    //            //console.log(result);
    //        }, function(response) {
    //            console.error("trouble\n" + JSON.parse(response));
    //            vm.isLoading = false;
    //        });
    //    }
    //}
    function loadUserData() {
        vm.isLoading = true; // Set to true when data loading starts
        UserDataService.GetUserData().then(function (result) {
            vm.UserData = result.data;
            // Additional data processing, if needed
        }).catch(function (error) {
            console.error("Error: " + JSON.stringify(error));
        }).finally(function () {
            vm.isLoading = false; // Set to false when data loading is complete (success or error)
        });
    }

}
