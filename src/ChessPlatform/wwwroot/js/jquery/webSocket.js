"use strict";

var Socket = function (webSocketService) {
    var websocket = new WebSocket((window.location.protocol === "https:" ? "wss://" : "ws://") + window.location.host);
    var service = webSocketService;

    websocket.onopen = function (e) { }

    websocket.onclose = function (e) { }

    websocket.onerror = function (e) { }

    websocket.onmessage = function (e) {
        service.onReceive(e.data);
    }

    this.sendMessage = function (message) {
        if (websocket.readyState != 1) {
            waitForSocketConnection(websocket, function () {
                websocket.send(message);
            });
        }
        else {
            websocket.send(message);
        }
    }

    function waitForSocketConnection(socket, callback) {
        setTimeout(function () {
            if (socket.readyState === 1) {
                if (callback != null) {
                    callback();
                }
                return;
            } else {
                waitForSocketConnection(socket, callback);
            }
        }, 100);
    }
}