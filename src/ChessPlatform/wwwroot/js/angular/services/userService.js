"use strict";

angular.module("userServices", []);

angular.module("userServices").service("userService", function ($http) {
    this.getPlayer = function (externalId) {
        return $http.get('/User/GetPlayer?externalId=' + externalId).then(function (response) {
            return {
                player: response.data.player,
                error: response.data.error,
                errorMessage: response.data.errorMessage
            }
        });
    }

    this.getCurrentPlayer = function () {
        return $http.get('/User/GetCurrentPlayer').then(function (response) {
            return {
                player: response.data.player,
                error: response.data.error,
                errorMessage: response.data.errorMessage
            }
        });
    }

    this.getPlayers = function () {
        return $http.get('/User/GetPlayers').then(function (response) {
            return {
                players: response.data.players,
                error: response.data.error,
                errorMessage: response.data.errorMessage
            }
        });
    }

    this.updateProfilePhoto = function (data) {
        return $http({
            method: 'POST',
            url: '/User/UpdateProfilePhoto',
            data: data,
            headers: { 'Content-Type': undefined }
        }).then(function (response) {
            return {
                error: response.data.error,
                errorMessage: response.data.errorMessage
            }
        });
    }
});