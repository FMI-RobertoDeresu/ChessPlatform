"use strict";

angular.module("gameServices", []);

angular.module("gameServices").service('gameService', function ($http) {
    this.getGame = function (id) {
        return $http.get('/game/getGame?id=' + id).then(function (response) {
            return {
                currentUserIsPlayer: response.data.currentUserIsPlayer,
                gameInfo: response.data.gameInfo,
                error: response.data.error,
                errorMessage: response.data.errorMessage
            };
        }, function (error) {
            return {
                error: true,
                errorMessage: error
            };
        });
    }

    this.getGames = function () {
        return $http.get("/game/getGames").then(function (response) {
            return {
                games: response.data.games,
                error: response.data.error,
                errorMessage: response.data.errorMessage
            };
        }, function (error) {
            return {
                error: true,
                errorMessage: error
            };
        });
    }

    this.playGame = function (id, password) {
        return $http.post("/game/play", { id: id, password: password }).then(function (response) {
            return {
                error: response.data.error,
                errorMessage: response.data.errorMessage
            };
        }, function (error) {
            return {
                error: true,
                errorMessage: error
            };
        })
    };
});