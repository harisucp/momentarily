angular
    .module('MomentarilyApp')
    .controller('PinPaymentSelectCardController', PinPaymentSelectCardController);
PinPaymentSelectCardController.$inject = ['$scope', '$document', 'Keys', 'Cards', 'Countries'];
function PinPaymentSelectCardController($scope, $document, Keys, Cards, Countries) {

    var vm = this;

    vm.submitForm = submitForm;
    vm.Errors = [];
    vm.Card = {};
    vm.ExpireMonths = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
    //vm.ExpireYears = [];
    vm.Countries = Countries;
    //test data
    //vm.Card = {
    //    //number: '4200000000000000',
    //    number: '4100000000000001',
    //    name: 'Test test',
    //    expiry_month: '05',
    //    expiry_year: '2019',
    //    cvc: '123',
    //    address_line1: 'Address 1',
    //    address_line2: 'Address 2',
    //    address_city: 'City',
    //    address_state: 'State',
    //    address_postcode: 'Post code',
    //    address_country: 'Australia'
    //};
    
    vm.UserCards = Cards;
    vm.isPaying = false;
    //vm.UserCards = [{DisplayNumber:"123", CardToken:"123"}];
    setValues();


    function setValues() {
        $scope.$watch('selectCard.SelectedCard', function (newValue, oldValue) {
            if (newValue === undefined || newValue === null) {
                vm.IsNewCard = true;
                vm.CardToken = "";
            } else {
                vm.CardToken = newValue;
                vm.IsNewCard = false;
            }
            //alert(newValue);
        });
        generateYears();
    }

    function generateYears() {
        var startYear = (new Date()).getFullYear();
        var maxYear = 30;
        var result = [];
        for (var i = startYear; i < (startYear + maxYear) ; i++) {
            result.push(i.toString());
        }
        vm.ExpireYears = result;
    }

    function submitForm($event, form) {
        if (form.$invalid) {
            $event.preventDefault();
            form.$submitted = true;
        } else {
            $event.preventDefault();
            vm.isPaying = true;
            //create card token
            if (vm.IsNewCard) {
                var pinApi = new Pin.Api(Keys.Key, Keys.Mode);
                vm.Errors = [];
                pinApi.createCardToken(vm.Card).then(handleSuccess, handleError).done();
            } else {
                $document.find("form")[0].submit();
            }

        }
    }

    function handleSuccess(card) {
        console.log(card);
        vm.CardToken = card.token;
        vm.DisplayNumber = card.display_number;
        vm.IsNewCard = true;
        $scope.$apply();
        $document.find("form")[0].submit();
        
        
        return;
        // Add the card token to the form
        //
        // Once you have the card token on your server you can use your
        // private key and Charges API to charge the credit card.
        $('<input>')
          .attr({ type: 'hidden', name: 'card_token' })
          .val(card.token)
          .appendTo(form);

        // Resubmit the form to the server
        //
        // Only the card_token will be submitted to your server. The
        // browser ignores the original form inputs because they don't
        // have their 'name' attribute set.
        form.get(0).submit();
    }


    function handleError(response) {
        angular.forEach(response.messages, function (key, value) {
            
            vm.Errors.push(key.message);
        });
        console.error(vm.Errors);
        vm.isPaying = false;
        $scope.$apply();
        //errorHeading.text(response.error_description);

        //if (response.messages) {
        //    $.each(response.messages, function (index, paramError) {
        //        $('<li>')
        //          .text(paramError.param + ": " + paramError.message)
        //          .appendTo(errorList);
        //    });
        //}

        //errorContainer.show();

        // Re-enable the submit button
        //submitButton.removeAttr('disabled');
    };
}