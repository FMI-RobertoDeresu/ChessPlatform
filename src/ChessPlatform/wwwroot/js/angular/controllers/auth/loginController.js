"use strict";

angular.module("authApp")
    .controller("loginController", loginController);

function loginController($http, $window) {
    var vm = this;

    vm.user = {};
    vm.errorMessage = null;
    vm.isBusy = true;
    vm.loggedIn = false;

    vm.login = function Login() {
        vm.errorMessage = null;
        vm.isBusy = true;

        $http.post('/auth/login', vm.user)
            .then(function (response) {
                if (response.data.authenticated == true) {
                    vm.loggedIn = true
                }
                else {
                    vm.errorMessage = response.data.error;
                }
            }, function (error) { })
            .finally(function () {
                vm.user = {};
                vm.isBusy = false;
            });
    };
};