angular
    .module('MomentarilyApp')
    .controller('RegisterController', RegisterController);

RegisterController.$inject = [];

function RegisterController() {

    var vm = this;

    vm.minDate = moment().subtract(18, 'year').format('MM/DD/YYYY');
}

    function showHideFunctionality() {
    var x = document.getElementById("Password");
    var y = document.getElementById("btnShowHide");
    if (x.type === "password") {
        x.type = "text";
        y.innerText = "HIDE";
    } else {
        x.type = "password";
        y.innerText = "SHOW";
    }
}


    function showHideFunctionalityInLogin() {
        var x = document.getElementById("ViewModel_Password");
        var y = document.getElementById("btnShowHide");
        if (x.type === "password") {
        x.type = "text";
         y.innerText = "HIDE";
        } else {
        x.type = "password";
    y.innerText = "SHOW";
        }
    }


    function showHideFunctionalityInReset1() {
        var x = document.getElementById("ViewModel_NewPassword");
        var y = document.getElementById("btnShowHide1");
        if (x.type === "password") {
            x.type = "text";
            y.innerText = "HIDE";
        } else {
            x.type = "password";
            y.innerText = "SHOW";
        }
    }


    function showHideFunctionalityInReset2() {
        var x = document.getElementById("ViewModel_ConfirmPassword");
        var y = document.getElementById("btnShowHide2");
        if (x.type === "password") {
            x.type = "text";
            y.innerText = "HIDE";
        } else {
            x.type = "password";
            y.innerText = "SHOW";
        }
}

    function showHideFunctionalityInchangeOldPwd() {
    
    var x = document.getElementById("OldPassword");
    var y = document.getElementById("btnShowHide1");
    if (x.type === "password") {
        x.type = "text";
        y.innerHTML = "<i class='fa fa-eye' aria-hidden='true'></i>";

    } else {
        x.type = "password";
        y.innerHTML = "<i class='fa fa-eye-slash' aria-hidden='true'></i>";
    }
    }

    function showHideFunctionalityInchangenewPwdInAdmin() {
    var x = document.getElementById("NewPassword");
    var y = document.getElementById("btnShowHide2");
    if (x.type === "password") {
        x.type = "text";
        y.innerHTML = "<i class='fa fa-eye' aria-hidden='true'></i>";

    } else {
        x.type = "password";
        y.innerHTML = "<i class='fa fa-eye-slash' aria-hidden='true'></i>";
    }
}
    
    function showHideFunctionalityInchangeconfirmPwdInAdmin() {
    var x = document.getElementById("ConfirmPassword");
    var y = document.getElementById("btnShowHide3");
    if (x.type === "password") {
        x.type = "text";
        y.innerHTML = "<i class='fa fa-eye' aria-hidden='true'></i>";
    } else {
        x.type = "password";
        y.innerHTML = "<i class='fa fa-eye-slash' aria-hidden='true'></i>";
    }
}

