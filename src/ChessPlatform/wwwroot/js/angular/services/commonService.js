"use strict";

angular.module("commonServices", []);

angular.module("commonServices").service("notificationService", function ($uibModal) {
    this.onNotification = function (category, message) {
        $uibModal.open({
            animation: true,
            templateUrl: '/views/common/notificationModal.html',
            controller: 'notificationModalController',
            resolve: {
                category: function () {
                    return category;
                },
                message: function () {
                    return message;
                }
            }
        });
    }
});

angular.module("commonServices").controller('notificationModalController', function ($scope, $uibModalInstance, category, message) {
    $scope.title = category;
    $scope.message = message;
    $scope.ok = function () {
        $uibModalInstance.dismiss();
    }
});