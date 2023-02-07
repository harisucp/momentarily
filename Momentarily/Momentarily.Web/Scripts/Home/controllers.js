angular
    .module('MomentarilyApp')
    .controller('HomeController', HomeController);
HomeController.$inject = ['Categories', '$location', '$anchorScroll'];
function HomeController(Categories, $location, $anchorScroll) {
    //var player_1 = new Vimeo.Player(document.getElementById('video_1')),
    //    player_2 = new Vimeo.Player(document.getElementById('video_2')),
    //pauseButtons = Array.from(document.getElementsByClassName("pause-btn"));

    var vm = this;

    vm.format = 'MM/DD/YYYY';
    vm.viewFormat = 'MM/DD/YYYY';
    vm.currentDate = moment().format(vm.format);
    vm.submitForm = submitForm;
    vm.scrolltoContent = scrolltoContent;

    //For comment IE
    //vm.videos = Array.from(document.getElementsByClassName("video"));
    //vm.textslides = Array.from(document.getElementsByClassName("text_slide"));

    vm.selectCategory = selectCategory;

    setValues();
    //pauseButtons.forEach(function (element) {
    //    element.addEventListener("click", pauseVideos);
    //});


    vm.findlocation = findMe;

    function findMe($event, form) {
        debugger
        setLoading('loadingItems', true);

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition);
            //$event.preventDefault();
            form.$invalid = false;
        } else {

            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    }

    function showPosition(position) {
        
        //x.innerHTML = "Latitude: " + position.coords.latitude +
        //    "<br>Longitude: " + position.coords.longitude;
        //var lat = position.coords.latitude;
        vm.location.Location.latitude = position.coords.latitude;
        vm.location.Location.longitude = position.coords.longitude;
        vm.location.Location.Latitude = position.coords.latitude;
        vm.location.Location.Longitude = position.coords.longitude;

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
                    
                    vm.location.Name = results[0].formatted_address;
                    $("#setlocation").val('');
                    $("#setlocation").val(results[0].formatted_address);
                }
            }
        });

    }

    function setLoading(nameObject, value) {
        if (nameObject && typeof (nameObject) === "string" && typeof (value) === "boolean") {
            vm[nameObject] = value;
        }
    }

    function setValues() {
        vm.availableItemTypes = Categories;


        vm.location = {
            Name: '',
            Location: {}
        };
    }

    function submitForm($event, form) {
        //debugger
        //console.log("2");
       
        //if (form.$invalid) {
        //    $event.preventDefault();
        //    form.$submitted = true;
        //} else {
        //    console.log(form);
        //}
    }

    function scrolltoContent() {
        $location.hash('block-with-icons');
        $anchorScroll();
    }

    //function pauseVideos() {
    //    player_1.pause();
    //    player_2.pause();
    //}

    function selectCategory(categoryId) {
        vm.itemTypeSelected = categoryId;
    }
}