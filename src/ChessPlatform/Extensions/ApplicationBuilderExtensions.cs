using System.Linq;
using ChessPlatform.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;

namespace ChessPlatform.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureNLog(this IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, string connectionString)
        {
            var nlogConfig = env.ConfigureNLog("nlog.config");
            var dbTargets = nlogConfig.AllTargets
                .Where(x => x is DatabaseTarget)
                .Cast<DatabaseTarget>()
                .ToList();

            dbTargets.ForEach(dbTarget => dbTarget.ConnectionString = connectionString);
            LogManager.ReconfigExistingLoggers();
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            return app;
        }

        public static IApplicationBuilder UseChessWebSocket(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ChessWebSocketMiddleware>();
        }
    }
}