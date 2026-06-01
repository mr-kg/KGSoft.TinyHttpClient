using System;
using Microsoft.Extensions.Logging;
using MicrosoftLogger = Microsoft.Extensions.Logging.ILogger;

namespace KGSoft.TinyHttpClient.Logging;

internal sealed class MicrosoftExtensionsLoggerAdapter : ILogger
{
    private readonly MicrosoftLogger _logger;

    public MicrosoftExtensionsLoggerAdapter(MicrosoftLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogMessage(string message)
    {
        _logger.Log(LogLevel.Information, new EventId(), message, null, (state, exception) => state);
    }

    public void LogException(Exception ex)
    {
        _logger.Log(LogLevel.Error, new EventId(), ex.Message, ex, (state, exception) => state);
    }
}
