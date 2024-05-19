namespace FluentDebug.Tests;

public class FluentDebugTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void log_sync_method_execution_time()
    {
        var result = FluentDebug.Create(new ConsoleLogger())
            .Run(() => DelayMethod(2));
    }

    [Test]
    public async Task log_async_method_execution_time()
    {
        var result = await FluentDebug.Create(new ConsoleLogger())
            .RunAsync(() => DelayMethodAsync(2));
    }

    private bool DelayMethod(int seconds)
    {
        Thread.Sleep(seconds * 1000);

        return true;
    }

    private async Task<bool> DelayMethodAsync(int seconds)
    {
        await Task.Delay(seconds * 1000);

        return true;
    }
}