"use strict";

(function() {
    angular.module("authApp")
        .controller("registerController", registerController);

    function registerController($http, $window, DEFAULT_URL) {
        var vm = this;

        vm.defaultUrl = DEFAULT_URL;

        vm.user = {};
        vm.errorMessage = null;

        vm.register = function register() {
            vm.errorMessage = null;
            vm.isBusy = true;

            $http.post(`${DEFAULT_URL}auth/register`, vm.user)
                .then(function(response) {
                    if (response.data.registered == true) {
                        vm.registered = true;
                    }
                    else {
                        vm.errorMessage = response.data.error;
                    }
                }, function(error) {})
                .finally(function() {
                    vm.isBusy = false;
                });
        };
    };
})();