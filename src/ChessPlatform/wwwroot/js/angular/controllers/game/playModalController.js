"use strict";

(function() {
    angular.module("gameApp").controller("playModalController", function($scope, $uibModalInstance, gameService, id) {
        $scope.ok = function() {
            gameService.playGame(id, $scope.password).then(function(response) {
                if (response.error === false) {
                    $uibModalInstance.close();
                }
                else {
                    $scope.password = "";
                    $scope.errorMessage = response.errorMessage;
                }
            });
        };

        $scope.cancel = function() {
            $uibModalInstance.dismiss();
        };
    });
})();