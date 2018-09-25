"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("rankingApp", ["ngRoute", "ui.bootstrap", "ngTable", "gameServices", "userServices", "commonServices"])
        .constant("DEFAULT_URL", defaultUrl)
        .config(function($routeProvider) {
            $routeProvider.when("/", {
                controller: "rankingController",
                templateUrl: `${defaultUrl}views/ranking/players.html`
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        });
})();