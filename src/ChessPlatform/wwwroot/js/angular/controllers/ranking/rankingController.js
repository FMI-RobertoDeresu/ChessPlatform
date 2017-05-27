"use strict";

angular.module('rankingApp').controller('rankingController', function ($scope, NgTableParams, userService, gameService) {
    userService.getPlayers().then(function (response) {
        if (!response.error) {
            $scope.pagedPlayers = new NgTableParams({}, { filterOptions: { filterFn: playersFilter }, dataset: response.players });
        }
        else {
            notificationService.onNotification('Error', response.errorMessage);
        }
    });

    var playersFilter = function () {


    }
});