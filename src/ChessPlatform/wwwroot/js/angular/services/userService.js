"use strict";

(function() {
    const defaultUrl = document.getElementById("default-url").value;

    angular.module("userServices", []).constant("DEFAULT_URL", defaultUrl);

    angular.module("userServices").service("userService", function($http, DEFAULT_URL) {
        this.getPlayer = function(externalId) {
            return $http.get(`${DEFAULT_URL}User/GetPlayer?externalId=` + externalId).then(function(response) {
                return {
                    player: response.data.player,
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            });
        };

        this.getCurrentPlayer = function() {
            return $http.get(`${DEFAULT_URL}User/GetCurrentPlayer`).then(function(response) {
                return {
                    player: response.data.player,
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            });
        };

        this.getPlayers = function() {
            return $http.get(`${DEFAULT_URL}User/GetPlayers`).then(function(response) {
                return {
                    players: response.data.players,
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            });
        };

        this.updateProfilePhoto = function(data) {
            return $http({
                method: "POST",
                url: `${DEFAULT_URL}User/UpdateProfilePhoto`,
                data: data,
                headers: { 'Content-Type': undefined }
            }).then(function(response) {
                return {
                    error: response.data.error,
                    errorMessage: response.data.errorMessage
                };
            });
        };
    });
})();