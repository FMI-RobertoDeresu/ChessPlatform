using System;
using NLog;

namespace ChessPlatform.Logging
{
    public class ApplicationLogger : IApplicationLogger
    {
        private readonly ILogger _logger;

        public ApplicationLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void LogError(Exception exception)
        {
            var errorLog = LogEventInfoFromException(exception, LogLevel.Error);
            _logger.Log(errorLog);
        }

        public void LogFatal(Exception exception)
        {
            var fatalLog = LogEventInfoFromException(exception, LogLevel.Fatal);
            _logger.Log(fatalLog);
        }

        private static LogEventInfo LogEventInfoFromException(Exception exception, LogLevel level)
        {
            var exceptionLog = new LogEventInfo(level, "ApplicationLogger", string.Empty);
            exceptionLog.Properties["Created"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            exceptionLog.Properties["Type"] = $"{level} - {exception.GetType().Name}";
            exceptionLog.Properties["Message"] = exception.Message;
            exceptionLog.Properties["Trace"] = exception.StackTrace;
            return exceptionLog;
        }
    }
}