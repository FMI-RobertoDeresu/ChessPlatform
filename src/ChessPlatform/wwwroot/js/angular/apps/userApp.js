"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("userApp", ["ngRoute", "userServices", "commonServices"])
        .constant("DEFAULT_URL", defaultUrl)
        .config(function($routeProvider) {
            $routeProvider.when("/", {
                controller: "userController",
                templateUrl: `${defaultUrl}views/user/profile.html`
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        });
})();