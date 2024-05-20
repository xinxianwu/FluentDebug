using FluentDebug.Loggers;

namespace FluentDebug.Tests;

public class FluentDebuggerTests
{
    private ILogAdapter _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new LogAdapter(new ConsoleLogger());
    }

    [Test]
    public void log_sync_method_execution_time()
    {
        var result = FluentDebugger.Create(_logger)
            .Run(() => DelayMethod(100));
    }

    [Test]
    public async Task log_async_method_execution_time()
    {
        var result = await FluentDebugger.Create(_logger)
            .RunAsync(() => DelayMethodAsync(100));
    }

    [Test]
    public void log_sync_method_parameters()
    {
        var result = FluentDebugger.Create(_logger)
            .LogParameters()
            .Run(() => DelayMethod(100));
    }
    
    
    [Test]
    public async Task log_async_method_parameters()
    {
        var result = await FluentDebugger.Create(_logger)
            .LogParameters()
            .RunAsync(() => DelayMethodAsync(100));
    }

    private bool DelayMethod(int milliseconds)
    {
        Thread.Sleep(milliseconds);

        return true;
    }

    private async Task<bool> DelayMethodAsync(int millisecond)
    {
        await Task.Delay(millisecond);

        return true;
    }
}