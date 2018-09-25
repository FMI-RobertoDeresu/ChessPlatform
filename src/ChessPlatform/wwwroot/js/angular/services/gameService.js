"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("gameServices", []).constant("DEFAULT_URL", defaultUrl);

    angular.module("gameServices").service("gameService", function($http, DEFAULT_URL) {
        this.getGame = function(id) {
            return $http.get(`${DEFAULT_URL}game/getGame?id=` + id).then(function(response) {
                return {
                    currentUserIsPlayer: response.data.currentUserIsPlayer,
                    gameInfo: response.data.gameInfo,
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            }, function(error) {
                return {
                    error: true,
                    errorMessage: error
                };
            });
        };

        this.getGames = function() {
            return $http.get(`${DEFAULT_URL}game/getGames`).then(function(response) {
                return {
                    games: response.data.games,
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            }, function(error) {
                return {
                    error: true,
                    errorMessage: error
                };
            });
        };

        this.playGame = function(id, password) {
            return $http.post(`${DEFAULT_URL}game/play`, { id: id, password: password }).then(function(response) {
                return {
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            }, function(error) {
                return {
                    error: true,
                    errorMessage: error
                };
            });
        };
    });
})();