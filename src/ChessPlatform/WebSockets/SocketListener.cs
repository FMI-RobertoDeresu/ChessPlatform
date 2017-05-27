using ChessPlatform.WebSockets.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ChessPlatform.WebSockets
{
    public class SocketListener
    {
        public static readonly ConcurrentDictionary<WebSocket, string> Chats = new ConcurrentDictionary<WebSocket, string>();
        public static readonly ConcurrentDictionary<WebSocket, string> Games = new ConcurrentDictionary<WebSocket, string>();

        public static async Task Handle(WebSocket webSocket)
        {
            var listener = new SocketListener(webSocket);
            await listener.Listen();
        }

        readonly WebSocket _webSocket;

        SocketListener(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public async Task Listen()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var error = string.Empty;

                try
                {
                    var buffer = new ArraySegment<Byte>(new Byte[4096]);
                    var received = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    switch (received.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            await OnDisconnect(_webSocket);
                            return;
                        case WebSocketMessageType.Text:
                        case WebSocketMessageType.Binary:
                            var data = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');
                            await OnReceive(_webSocket, data);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    error = exception.ToString();
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    await Send(_webSocket, new ErrorModel(error));
                }
            }
        }

        static async Task OnReceive(WebSocket socket, string data)
        {
            switch ((JsonConvert.DeserializeObject<BaseModel>(data)).MessageType)
            {
                case "join":
                    await OnJoin(socket, JsonConvert.DeserializeObject<JoinModel>(data));
                    break;
                case "message":
                    await OnMessage(socket, JsonConvert.DeserializeObject<MessageModel>(data));
                    break;
                case "start":
                    await OnStartGame(JsonConvert.DeserializeObject<BaseModel>(data));
                    break;
                case "move":
                    await OnMove(socket, JsonConvert.DeserializeObject<MovelModel>(data));
                    break;
            }
        }

        static async Task OnJoin(WebSocket socket, JoinModel joinModel)
        {
            switch (joinModel.Type)
            {
                case "game":
                    joinModel.Succeded = Games.TryAdd(socket, joinModel.RoomId);
                    break;
                case "chat":
                    joinModel.Succeded = Chats.TryAdd(socket, joinModel.RoomId);
                    break;
                case "all":
                    joinModel.Succeded = Games.TryAdd(socket, joinModel.RoomId) && Chats.TryAdd(socket, joinModel.RoomId);
                    break;
            }

            if (!joinModel.Succeded)
            {
                string value;

                Chats.TryRemove(socket, out value);
                Games.TryRemove(socket, out value);
            }

            await Send(socket, joinModel);
        }

        static async Task OnMessage(WebSocket socket, MessageModel messageModel)
        {
            ConcurrentDictionary<WebSocket, string> addresses = new ConcurrentDictionary<WebSocket, string>(
                        Chats.Where(x => x.Key != socket && x.Value == messageModel.RoomId).Select(x => new KeyValuePair<WebSocket, string>(x.Key, x.Value)));

            await Broadcast(addresses, messageModel);
            await Send(socket, new MessageModel(messageModel, true));
        }

        static async Task OnStartGame(BaseModel model)
        {
            ConcurrentDictionary<WebSocket, string> addresses = new ConcurrentDictionary<WebSocket, string>(
                        Games.Where(x => x.Value == model.RoomId).Select(x => new KeyValuePair<WebSocket, string>(x.Key, x.Value)));

            await Broadcast(addresses, model);
        }

        static async Task OnMove(WebSocket socket, MovelModel moveModel)
        {
            ConcurrentDictionary<WebSocket, string> addresses = new ConcurrentDictionary<WebSocket, string>(
                        Games.Where(x => x.Key != socket && x.Value == moveModel.RoomId).Select(x => new KeyValuePair<WebSocket, string>(x.Key, x.Value)));

            await Broadcast(addresses, moveModel);
        }

        static async Task Broadcast<T>(ConcurrentDictionary<WebSocket, string> addresses, T message)
        {
            await Task.WhenAll(addresses.Where(x => x.Key.State == WebSocketState.Open).Select(x => Send(x.Key, message)));
        }

        static async Task Send<T>(WebSocket socket, T response)
        {
            var text = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            var data = Encoding.UTF8.GetBytes(text);
            var buffer = new ArraySegment<Byte>(data);

            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task OnDisconnect(WebSocket socket)
        {
            string value;

            Chats.TryRemove(socket, out value);
            Games.TryRemove(socket, out value);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reasons", CancellationToken.None);
        }
    }
}
