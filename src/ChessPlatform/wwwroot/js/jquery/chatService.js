"use strict";

var ChatService = function (webSocketService, name) {
    var socketService = webSocketService;
    var name = name;
    const defaultUrl = document.getElementById("default-url").value;

    var $chat = $("#game-chat-container");
    var $messagesList = $chat.find(".messages-list");

    this.sendMessage = function (gameId, name, message) {
        var id = guid();

        webSocketService.sendMessage(gameId, id, name, message);
        addMessage(id, name, message, true);
    }

    this.receiveMessage = function (message) {
        if (!message.mine) {
            addMessage(message.id, message.from, message.content, false);
        }
        else {
            disableSendingImg(message.id);
        }
    }

    function addMessage(id, from, content, myMessage) {
        var message = [];

        message.push("<li id='" + id + "' class='message " + (myMessage ? 'right' : '') + "'>");

        if (!myMessage) {
            message.push("<span class='from'>" + from + "</span>");
        }

        message.push("<div>")

        if (myMessage) {
            message.push(`<img src='${defaultUrl}img/sending-message.gif'/>`);
        }

        message.push("<span class='content'>" + content + "</span>");

        message.push("</div>")
        message.push("</li>");

        $messagesList.append(message.join(""));
        //best shit ever
        //this must be temporary
        //change this - ul height not working
        $messagesList.animate({ scrollTop: 100000 }, 100);
    }

    function disableSendingImg(id) {
        $chat.find('#' + id).find('img').remove();
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        }

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }
}