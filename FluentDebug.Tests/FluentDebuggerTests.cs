using FluentAssertions;
using FluentDebug.Loggers;
using FluentDebug.Tests.Exceptions;
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

        LogContextShouldContainSubString("Execution time:");
    }

    [Test]
    public async Task log_async_method_execution_time()
    {
        var result = await FluentDebugger.Create(_logger)
            .RunAsync(() => DelayMethodAsync(100));

        LogContextShouldContainSubString("Execution time:");
    }

    [Test]
    public void log_sync_method_parameters()
    {
        var result = FluentDebugger.Create(_logger)
            .LogParameters()
            .Run(() => DelayMethod(100,
                new List<int> { 1, 2, 3 },
                new Dictionary<string, int>
                {
                    { "a", 1 },
                    { "b", 2 },
                    { "c", 3 }
                }));

        LogContextShouldContainSubString("[DelayMethod()] Parameters: `milliseconds: 100, list: [1, 2, 3], dictionary: {a: 1, b: 2, c: 3}` | Execution time:");
    }

    [Test]
    public async Task log_async_method_parameters()
    {
        var result = await FluentDebugger.Create(_logger)
            .LogParameters()
            .RunAsync(() => DelayMethodAsync(100, new List<int> { 1, 2, 3 }));

        LogContextShouldContainSubString("[DelayMethodAsync()] Parameters: `milliseconds: 100, list: [1, 2, 3]` | Execution time:");
    }

    [Test]
    public void throw_exception_in_sync_method()
    {
        var action = () =>
        {
            FluentDebugger.Create(_logger)
                .LogParameters()
                .Rethrow(exception => new MyException(exception, "An error occurred"))
                .Run(() => ThrowException(100,
                    new List<int> { 1, 2, 3 },
                    new Dictionary<string, int>
                    {
                        { "a", 1 },
                        { "b", 2 },
                        { "c", 3 }
                    }));
        };

        action.Should().Throw<MyException>()
            .WithMessage("An error occurred")
            .WithInnerException<ArgumentException>()
            .WithMessage("Too many arguments");
    }

    [Test]
    public void throw_exception_in_async_method()
    {
        var action = () =>
        {
            FluentDebugger.Create(_logger)
                .LogParameters()
                .Rethrow(exception => new MyException(exception, "An error occurred"))
                .RunAsync(() => ThrowExceptionAsync(100,
                    new List<int> { 1, 2, 3 },
                    new Dictionary<string, int>
                    {
                        { "a", 1 },
                        { "b", 2 },
                        { "c", 3 }
                    }))
                .GetAwaiter()
                .GetResult();
        };

        action.Should().Throw<MyException>()
            .WithMessage("An error occurred")
            .WithInnerException<ArgumentException>()
            .WithMessage("Too many arguments");
    }

    private bool ThrowException(int milliseconds, List<int> list, Dictionary<string, int> dictionary)
    {
        throw new ArgumentException("Too many arguments");

        return true;
    }

    private async Task<bool> ThrowExceptionAsync(int milliseconds, List<int> list, Dictionary<string, int> dictionary)
    {
        await Task.Delay(1);
        
        throw new ArgumentException("Too many arguments");

        return true;
    }

    private void LogContextShouldContainSubString(string subString)
    {
        _logger.Received(1).Log(Arg.Is<string>(s => s.Contains(subString)));
    }

    private bool DelayMethod(int milliseconds, List<int> list, Dictionary<string, int> dictionary)
    {
        Thread.Sleep(milliseconds);

        return true;
    }

    private bool DelayMethod(int milliseconds)
    {
        Thread.Sleep(milliseconds);

        return true;
    }

    private async Task<object> DelayMethodAsync(int milliseconds, List<int> list)
    {
        await Task.Delay(milliseconds);

        return true;
    }

    private async Task<bool> DelayMethodAsync(int milliseconds)
    {
        await Task.Delay(milliseconds);

        return true;
    }
}