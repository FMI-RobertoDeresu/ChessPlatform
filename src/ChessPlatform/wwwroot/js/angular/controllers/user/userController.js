"use strict";

(function() {
    angular.module("userApp").controller("userController", function($scope, userService) {
        userService.getPlayer($scope.externalId).then(function(response) {
            if (!response.error) {
                $scope.user = response.player;
            }
        });
    });
})();