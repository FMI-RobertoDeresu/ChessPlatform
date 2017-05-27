"use strict";

angular.module("userApp", ["ngRoute", "userServices", "commonServices"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "userController",
            templateUrl: "/views/user/profile.html"
        });

        $routeProvider.otherwise({ redirectTo: "/" });
    });