"use strict";

angular.module("gameApp", ["ngRoute", 'ngAnimate', 'ui.bootstrap', "ngTable", "gameServices", "userServices", "commonServices"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "gameController",
            templateUrl: "/views/game/gamesGrid.html"
        });

        $routeProvider.otherwise({ redirectTo: "/" });
    });