"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("authApp", ["ngRoute"])
        .constant("DEFAULT_URL", defaultUrl)
        .config(function($routeProvider) {
            $routeProvider.when("/", {
                controller: "loginController",
                controllerAs: "vm",
                templateUrl: `${defaultUrl}views/auth/login.html`
            });

            $routeProvider.when("/register", {
                controller: "registerController",
                controllerAs: "vm",
                templateUrl: `${defaultUrl}views/auth/register.html`
            });

            $routeProvider.otherwise({ redirecTo: "/" });
        });
})();