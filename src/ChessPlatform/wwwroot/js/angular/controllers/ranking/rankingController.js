"use strict";

(function() {
    angular.module("rankingApp")
        .controller("rankingController", rankingController);

    function rankingController($scope, NgTableParams, userService, notificationService, DEFAULT_URL) {
        $scope.defaultUrl = DEFAULT_URL;
        userService.getPlayers().then(function(response) {
            var playersFilter = function() {};

            if (!response.error) {
                $scope.pagedPlayers =
                    new NgTableParams({},
                        { filterOptions: { filterFn: playersFilter }, dataset: response.players });
            }
            else {
                notificationService.onNotification("Error", response.errorMessage);
            }
        });
    }
})();