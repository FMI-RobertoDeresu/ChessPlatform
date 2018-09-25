"use strict";

(function() {
    angular.module("authApp")
        .controller("loginController", loginController);

    function loginController($http, $window, DEFAULT_URL) {
        var vm = this;

        vm.defaultUrl = DEFAULT_URL;

        vm.user = {};
        vm.errorMessage = null;
        vm.isBusy = true;
        vm.loggedIn = false;

        vm.login = function Login() {
            vm.errorMessage = null;
            vm.isBusy = true;

            $http.post(`${DEFAULT_URL}auth/login`, vm.user)
                .then(function(response) {
                    if (response.data.authenticated == true) {
                        vm.loggedIn = true;
                    }
                    else {
                        vm.errorMessage = response.data.error;
                    }
                }, function(error) {})
                .finally(function() {
                    vm.user = {};
                    vm.isBusy = false;
                });
        };
    }
})();