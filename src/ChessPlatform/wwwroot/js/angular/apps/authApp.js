"use strict";

angular.module("authApp", ["ngRoute"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "loginController",
            controllerAs: "vm",
            templateUrl: "/views/auth/login.html"
        });

        $routeProvider.when("/register", {
            controller: "registerController",
            controllerAs: "vm",
            templateUrl: "/views/auth/register.html"
        });

        $routeProvider.otherwise({ redirecTo: "/" });
    });