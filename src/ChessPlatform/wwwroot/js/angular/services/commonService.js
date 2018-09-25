"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("commonServices", [])
        .constant("DEFAULT_URL", defaultUrl)
        .service("notificationService", notificationService)
        .controller("notificationModalController", notificationModalController);

    function notificationService($uibModal, DEFAULT_URL) {
        this.onNotification = function(category, message) {
            $uibModal.open({
                animation: true,
                templateUrl: `${DEFAULT_URL}views/common/notificationModal.html`,
                controller: "notificationModalController",
                resolve: {
                    category: function() {
                        return category;
                    },
                    message: function() {
                        return message;
                    }
                }
            });
        };
    }

    function notificationModalController($scope, $uibModalInstance, category, message) {
        $scope.title = category;
        $scope.message = message;
        $scope.ok = function() {
            $uibModalInstance.dismiss();
        };
    }
})();