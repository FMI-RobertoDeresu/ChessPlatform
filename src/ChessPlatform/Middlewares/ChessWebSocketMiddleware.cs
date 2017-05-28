using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace ChessPlatform.Middlewares
{
    public class ChessWebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public ChessWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            return _next(httpContext);
        }
    }
}