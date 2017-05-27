"use strict";

var WebSocketService = function () {
    var socket = new Socket(this);
    var gameService;
    var chatService;
    var joined;

    this.joinChatRoom = function (roomId) {
        this.sendObject({ roomId: roomId, messageType: 'join', type: 'chat' });
    }

    this.joinGameRoom = function (roomId) {
        this.sendObject({ roomId: roomId, messageType: 'join', type: 'game' });
    }

    this.joinChatAndGameRooms = function (roomId) {
        this.sendObject({ roomId: roomId, messageType: 'join', type: 'all' });
    }

    this.sendMessage = function (roomId, id, from, content) {
        this.sendObject({ roomId: roomId, id: id, from: from, content: content, messageType: 'message' });
    }

    this.startGame = function (roomId) {
        this.sendObject({ roomId: roomId, messageType: 'start' });
    }

    this.makeMove = function (roomId, move) {
        this.sendObject({ roomId: roomId, from: move.from, to: move.to, promotion: move.promotion, messageType: 'move' });
    }

    this.sendObject = function (object) {
        socket.sendMessage(JSON.stringify(object));
    }

    this.onReceive = function (data) {
        var object = JSON.parse(data);

        switch (object.messageType) {
            case "join":
                if (object.succeded) {
                    joined = object.type;
                }
                break;
            case "message":
                if (joined == "chat" || joined == "all") {
                    chatService.receiveMessage(object);
                }
                break;
            case "start":
                gameService.startGame();
                break;
            case "move":
                if (joined == "game" || joined == "all") {
                    gameService.makeMove(object, true);
                }
                break;
            case "error":
                console.log(object.errorMessage);
        }
    }

    this.initialize = function (igameService, ichatService) {
        gameService = igameService;
        chatService = ichatService;
    }
}