angular
    .module('MomentarilyApp')
    .controller('LayoutController', LayoutController);

LayoutController.$inject = ['$scope', '$interval', 'UserDataService'];

function LayoutController($scope, $interval, UserDataService) {
    var vm = this;
    vm.UserData = {};
    vm.hasUnreadMessages = hasUnreadMessages;
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

    function setValues() {
        loadUserData();
    }
    
    function loadUserData() {
        if (!vm.isLoading) {
            vm.isLoading = true;
            UserDataService.GetUserData().then(function(result) {
                vm.UserData = result.data;
                vm.isLoading = false;
                //console.log(result);
            }, function(response) {
                console.error("trouble\n" + JSON.parse(response));
                vm.isLoading = false;
            });
        }
    }

}