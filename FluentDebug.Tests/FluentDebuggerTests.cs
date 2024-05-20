using FluentDebug.Loggers;
using NSubstitute;

namespace FluentDebug.Tests;

public class FluentDebuggerTests
{
    private ILogAdapter _logger;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogAdapter>();
        // _logger = new LogAdapter(new ConsoleLogger());
    }

    [Test]
    public void log_sync_method_execution_time()
    {
        var result = FluentDebugger.Create(_logger)
            .Run(() => DelayMethod(100));

        _logger.Received(1).Log(Arg.Is<string>(s => s.Contains("Execution time:")));
    }

    [Test]
    public async Task log_async_method_execution_time()
    {
        var result = await FluentDebugger.Create(_logger)
            .RunAsync(() => DelayMethodAsync(100));

        _logger.Received(1).Log(Arg.Is<string>(s => s.Contains("Execution time:")));
    }

    [Test]
    public void log_sync_method_parameters()
    {
        var result = FluentDebugger.Create(_logger)
            .LogParameters()
            .Run(() => DelayMethod(100));

        _logger.Received(1).Log(Arg.Is<string>(s => s.Contains("[DelayMethod()] Parameters: `milliseconds: 100` | Execution time:")));
    }

    [Test]
    public async Task log_async_method_parameters()
    {
        var result = await FluentDebugger.Create(_logger)
            .LogParameters()
            .RunAsync(() => DelayMethodAsync(100));

        _logger.Received(1).Log(Arg.Is<string>(s => s.Contains("[DelayMethodAsync()] Parameters: `millisecond: 100` | Execution time:")));
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