"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("gameApp",
            ["ngRoute", "ngAnimate", "ui.bootstrap", "ngTable", "gameServices", "userServices", "commonServices"])
        .constant("DEFAULT_URL", defaultUrl)
        .config(function($routeProvider) {
            $routeProvider.when("/", {
                controller: "gameController",
                templateUrl: `${defaultUrl}views/game/gamesGrid.html`
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        });
})();