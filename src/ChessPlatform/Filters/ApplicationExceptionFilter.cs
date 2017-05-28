using ChessPlatform.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChessPlatform.Filters
{
    public class ApplicationExceptionFilter : IExceptionFilter
    {
        private readonly IApplicationLogger _appLogger;

        public ApplicationExceptionFilter(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public void OnException(ExceptionContext context)
        {
            _appLogger.LogFatal(context.Exception);
        }
    }
}