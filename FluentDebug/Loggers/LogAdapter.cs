using Microsoft.Extensions.Logging;

namespace FluentDebug.Loggers;

public class LogAdapter : ILogAdapter
{
    private readonly ILogger _logger;

    public LogAdapter(ILogger logger)
    {
        _logger = logger;
    }

    public void Log(string message)
    {
        _logger.LogInformation(message);
    }
}