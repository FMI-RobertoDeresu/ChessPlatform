(function () {
    "use strict";

    angular.module("rankingApp", ["ngRoute", "ngTable", "gameServices", "userServices"])
        .config(function ($routeProvider) {
            $routeProvider.when("/", {
                controller: "rankingController",
                templateUrl: "/views/ranking/players.html"
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        });
})();