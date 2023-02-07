angular
    .module('MomentarilyApp')
    .controller('UserProfileController', UserProfileController)
    .controller('UserBankInfoController', UserBankInfoController)
    .controller('UserPublicProfileController', UserPublicProfileController);

UserProfileController.$inject = ['profileData'];

function UserProfileController(profileData) {
    var vm = this;

    vm.profileData = profileData;
    vm.userImage = profileData.UserImage;


    function setValues() {

    };
}

UserBankInfoController.$inject = ['bankInfo'];

function UserBankInfoController(bankInfo) {
    var vm = this;
    vm.bankInfo = bankInfo;
    vm.submitForm = submitForm;
    setValues();

    function setValues() {
        
    }
    function submitForm($event, form) {
        if (form.$invalid) {
            $event.preventDefault();
            form.$submitted = true;
        }
    }
}


UserPublicProfileController.$inject = ['profileData', 'ReviewService'];

function UserPublicProfileController(profileData, ReviewService) {
    var vm = this;
    vm.profileData = profileData;
    vm.loadMoreSeekerReview = loadMoreSeekerReview;
    vm.loadMoreSharerReview = loadMoreSharerReview;
    vm.showMoreSeekerReview = false;
    vm.showModeSharerReview = false;
    vm.isSeekerLoading = false;
    vm.isSharerLoading = false;
    //vm.currentSeekerPage = profileData.
    
    console.log(profileData);
    setValues();
    function setValues() {
        vm.showMoreSeekerReview = vm.profileData.SeekersReviews.Items.length < vm.profileData.SeekersReviews.Pagination.TotalItems;
        vm.showModeSharerReview = vm.profileData.SharersReviews.Items.length < vm.profileData.SharersReviews.Pagination.TotalItems;

    }

    function loadMoreSeekerReview() {
        vm.isSeekerLoading = true;
        ReviewService.GetSeekersReview(vm.profileData.User.Id, vm.profileData.SeekersReviews.Pagination.CurrentPage + 1).then(function (result) {
            result.data.Items.forEach(function(item) {
                vm.profileData.SeekersReviews.Items.push(item);
            });
            vm.profileData.SeekersReviews.Pagination = result.data.Pagination;
            vm.showMoreSeekerReview = vm.profileData.SeekersReviews.Items.length < vm.profileData.SeekersReviews.Pagination.TotalItems;
            //angular.extend(vm.profileData.SeekersReviews.Items, result.data.Items);
            (result.data.Items.length === 0) ? vm.showMoreSeekerReview = false : vm.showMoreSeekerReview = true;
            console.log(result);
        }).then(function() {
            vm.isSeekerLoading = false;
        });
    }

    function loadMoreSharerReview() {
        vm.isSharerLoading = true;
        ReviewService.GetSharersReview(vm.profileData.User.Id, vm.profileData.SharersReviews.Pagination.CurrentPage + 1).then(function (result) {
            result.data.Items.forEach(function (item) {
                vm.profileData.SharersReviews.Items.push(item);
            });
            vm.profileData.SharersReviews.Pagination = result.data.Pagination;
            vm.showModeSharerReview = vm.profileData.SharersReviews.Items.length < vm.profileData.SharersReviews.Pagination.TotalItems;
            //angular.extend(vm.profileData.SeekersReviews.Items, result.data.Items);

            console.log(result);
        }).then(function() {
            vm.isSharerLoading = false;
        });
    }
}