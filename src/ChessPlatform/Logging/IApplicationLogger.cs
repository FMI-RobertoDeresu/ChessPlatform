using System;

namespace ChessPlatform.Logging
{
    public interface IApplicationLogger
    {
        void LogError(Exception exception);
        void LogFatal(Exception exception);
    }
}