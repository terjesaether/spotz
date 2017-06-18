$(function () {

    getLocation();

    var x = document.getElementById("showLocation");
    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition, showError);
        } else {
            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    }
    function showPosition(position) {
        //x.innerHTML = "Latitude: " + position.coords.latitude +
        //    "<br>Longitude: " + position.coords.longitude;

        ko.applyBindings(new ViewModel(position.coords.latitude, position.coords.longitude));
    }

    // Show error for getting location

    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                x.innerHTML = "User denied the request for Geolocation.";
                $('#noLocationAlert').show();
                break;
            case error.POSITION_UNAVAILABLE:
                x.innerHTML = "Location information is unavailable.";
                break;
            case error.TIMEOUT:
                x.innerHTML = "The request to get user location timed out.";
                break;
            case error.UNKNOWN_ERROR:
                x.innerHTML = "An unknown error occurred.";
                break;
        }
    }



    // KNOCKOUT
    function ViewModel(currLatitude, currLongitude) {
        var self = this;
        self.spotz = ko.observableArray([]);

        self.loading = ko.observable(true);

        console.log('Getting spotz');

        $.get('/api/getspotzesfromdistance/',
                { latitude: currLatitude, longitude: currLongitude })
            .done(function (data) {
                self.spotz.push.apply(self.spotz, data);
                self.loading(false);
                console.log(data);
            })
            .fail(function (err) {
                console.error(err);
            });

    };



});

