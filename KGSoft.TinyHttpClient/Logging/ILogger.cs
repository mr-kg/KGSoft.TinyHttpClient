using System;

namespace KGSoft.TinyHttpClient.Logging;

public interface ILogger
{
    void LogMessage(string message);
    void LogException(Exception ex);
}
