"use strict";

angular.module("gameApp").controller("gameController", function ($scope, $window, $uibModal, NgTableParams, gameService, userService, notificationService) {
    $scope.filter = [];

    gameService.getGames().then(function (response) {
        if (!response.error) {
            $scope.games = response.games;
            $scope.pagedGames = new NgTableParams({}, { dataset: response.games });
        }
        else {
            notificationService.onNotification('Error', response.errorMessage);
        }
    });

    userService.getCurrentPlayer().then(function (response) {
        if (!response.error) {
            $scope.player = response.player;
        }
        else {
            notificationService.onNotification('Error', response.errorMessage);
        }
    });

    $scope.joinGame = function (id) {
        $window.location.href = '/game?id=' + id;
    }

    $scope.spectateGame = function (event) {
        $scope.joinGame(parseInt(event.currentTarget.dataset["game"]));
    }

    $scope.playGame = function (event) {
        var id = parseInt(event.currentTarget.dataset["game"]);

        gameService.getGame(id).then(function (response) {
            if (!response.error) {
                if (response.gameInfo.started) {
                    $scope.joinGame(id);
                }
                else {
                    if (response.gameInfo.requirePassword == false || response.currentUserIsPlayer) {
                        gameService.playGame(id, null).then(function (response) {
                            if (response.error == false) {
                                $scope.joinGame(id);
                            }
                            else {
                                notificationService.onNotification('', response.errorMessage);
                            }
                        });
                    }
                    else {
                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: '/views/game/playModal.html',
                            controller: 'playModalController',
                            resolve: {
                                id: function () {
                                    return id;
                                }
                            }
                        });

                        modalInstance.result.then(function () {
                            $scope.joinGame(id);
                        });
                    }
                }
            }
            else {
                notificationService.onNotification('Error', response.errorMessage);
            }
        });
    }

    $scope.updateProfilePhotoModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/views/user/updateProfilePhotoModal.html',
            controller: 'updateProfilePhotoModalController'
        });

        modalInstance.result.then(function () {
            userService.getCurrentPlayer().then(function (response) {
                if (!response.error) {
                    $scope.player = response.player;
                }
            });
        });
    }

    $scope.gameFilter = function () {
        function includesLower(first, second) {
            if (first != null && second != null) {
                return _.includes(first.toLowerCase(), second.toLowerCase());
            }

            return false;
        }

        var filteredGames = $scope.games.filter(function (item) {
            if ($scope.filter.name != null && !includesLower(item.gameInfo.name, $scope.filter.name)) {
                return false;
            }

            if ($scope.filter.player != null && !includesLower(item.gameInfo.player1.profile.nickname, $scope.filter.player) && !includesLower(item.gameInfo.player2.profile.nickname, $scope.filter.player)) {
                return false;
            }

            if ($scope.filter.points != null && ((item.gameInfo.minimumNumberOfPoints != null && item.gameInfo.minimumNumberOfPoints > $scope.filter.points) || (item.gameInfo.maximumNumberOfPoints != null && item.gameInfo.maximumNumberOfPoints < $scope.filter.points))) {
                return false;
            }

            return true;
        });

        $scope.pagedGames = new NgTableParams({}, { dataset: filteredGames });
    };
});
