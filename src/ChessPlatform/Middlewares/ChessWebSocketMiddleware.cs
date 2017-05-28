using System.Net.WebSockets;
using System.Threading.Tasks;
using ChessPlatform.WebSockets;
using Microsoft.AspNetCore.Http;

namespace ChessPlatform.Middlewares
{
    public class ChessWebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public ChessWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                    await SocketListener.Handle(webSocket);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}