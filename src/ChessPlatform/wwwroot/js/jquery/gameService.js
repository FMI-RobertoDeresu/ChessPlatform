"use strict";

var GameService = function(webSocketService, gameModel) {
    this.socketService = webSocketService;
    this.gameModel = gameModel;
    this.chess = new Chess();
    var self = this;
    const defaultUrl = document.getElementById("default-url").value;

    var $notificationContainer = $(".notification-container");

    var removeGreySquares = function() {
        $("#board .square-55d63").css("background", "");
    };

    var greySquare = function(square) {
        var squareEl = $(`#board .square-${square}`);

        squareEl.css("background", squareEl.hasClass("black-3c85d") ? "#696969" : "#a9a9a9");
    };

    var onDragStart = function(source, piece) {
        if (self.chess.game_over() === true ||
            (self.chess.turn() === "w" && (piece.search(/^b/) !== -1 || !self.gameModel.currentUserIsPlayer1)) ||
            (self.chess.turn() === "b" && (piece.search(/^w/) !== -1 || !self.gameModel.currentUserIsPlayer2))) {
            return false;
        }
    };

    var onDrop = function(source, target) {
        removeGreySquares();

        var move = { from: source, to: target, promotion: "q" };

        if (self.isValidMove(move)) {
            var status = self.chess.fen();

            self.makeMove(move, false);
            self.postMove(move, function(success) {
                if (success) {
                    self.socketService.makeMove(self.gameModel.gameInfo.id, move);

                    if (self.chess.game_over()) {
                        self.onEnd(self.chess.in_checkmate());
                    }
                    else if (self.gameModel.gameInfo.withComputer && self.chess.turn() === "b") {
                        self.getComputerMove();
                    }
                }
                else {
                    self.chess.load(status);
                    self.board.position(status);
                }
            });
        }
        else {
            return "snapback";
        }
    };

    var onMouseoverSquare = function(square, piece) {
        var moves = self.chess.moves({
            square: square,
            verbose: true
        });

        if ((self.chess.turn() === "w" && !self.gameModel.currentUserIsPlayer1) ||
            (self.chess.turn() === "b" && !self.gameModel.currentUserIsPlayer2) ||
            moves.length === 0) {
            return;
        }

        greySquare(square);

        for (let i = 0; i < moves.length; i++) {
            greySquare(moves[i].to);
        }
    };

    var onMouseoutSquare = function(square, piece) {
        removeGreySquares();
    };

    var onSnapEnd = function() {};

    var initialize = function() {
        $(".chess-container").width(Math.min(window.innerWidth, window.innerHeight) - 50.75);
        $(".chess-container").height(Math.min(window.innerWidth, window.innerHeight) - 50.75);

        self.chess.load(self.gameModel.gameInfo.status);

        self.board = ChessBoard("board", {
            draggable: self.gameModel.currentUserIsPlayer,
            orientation: self.gameModel.currentUserIsPlayer2 ? "black" : "white",
            position: self.gameModel.gameInfo.status,
            onDragStart: onDragStart,
            onDrop: onDrop,
            onMouseoutSquare: onMouseoutSquare,
            onMouseoverSquare: self.gameModel.currentUserIsPlayer ? onMouseoverSquare : null,
            onSnapEnd: onSnapEnd
        });

        if (self.gameModel.currentUserIsPlayer2 && self.gameModel.gameInfo.turn == 0) {
            self.socketService.startGame(self.gameModel.gameInfo.id);
        }
    }();

    this.isValidMove = function(move) {
        if (self.gameModel.gameInfo.started && !gameModel.gameInfo.ended) {
            var status = self.chess.fen();

            if (self.chess.move({ from: move.from, to: move.to, promotion: move.promotion }) != null) {
                return self.chess.load(status);
            }
        }

        return false;
    };

    this.startGame = function() {
        if (!self.gameModel.gameInfo.started) {
            self.gameModel.gameInfo.started = true;
            $notificationContainer.css("display", "none");
        }
    };

    this.getComputerMove = function() {
        $.get(`${defaultUrl}game/getbestmove?fen=${self.chess.fen()}`, function(response) {
            onDrop(response.from, response.to);
        });
    };

    this.makeMove = function(move, fromSocket) {
        if (self.isValidMove(move)) {
            self.chess.move({ from: move.from, to: move.to, promotion: move.promotion });

            if (fromSocket || (self.gameModel.gameInfo.withComputer && self.chess.turn() === "w")) {
                self.board.position(self.chess.fen());

                if (fromSocket && self.chess.game_over()) {
                    self.onEnd(false);
                }
            }
        }
    };

    this.postMove = function(move, callback) {
        var makeMoveModel = {
            gameId: self.gameModel.gameInfo.id,
            boardStatus: self.chess.fen(),
            from: move.From,
            to: move.To,
            promotion: move.Promotion,
            endGame: self.chess.game_over(),
            isCheckmate: self.chess.in_checkmate()
        };

        $.post(`${defaultUrl}game/makeMove`, makeMoveModel, function(response) {
            callback(!response.Error);
        }).fail(function() {
            return callback(false);
        });
    };

    this.onEnd = function(winGame) {
        self.gameModel.gameInfo.ended = true;

        if (self.gameModel.currentUserIsPlayer) {
            if (!self.gameModel.gameInfo.withComputer) {
                $notificationContainer.find(".notification-message")
                    .html(winGame ? "You're the best!" : "Maybe next time.");
            }
            else {
                $notificationContainer.find(".notification-message")
                    .html(self.chess.turn() == "b" ? "You're the best!" : "Maybe next time.");
            }
        }
        else {
            if (!self.gameModel.gameInfo.withComputer) {
                $notificationContainer.find(".notification-message").html(
                    (self.chess.turn() == "w" ? self.gameModel.gameInfo.player2 : self.gameModel.gameInfo.player1) +
                    " has win this game!");
            }
            else {
                $notificationContainer.find(".notification-message").html(
                    (self.chess.turn() == "w" ? "Computer" : self.gameModel.gameInfo.player1) + " has win this game!");
            }
        }

        $notificationContainer.css("display", "block");
    };
};