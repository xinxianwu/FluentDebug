using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluentDebug
{
    public class FluentDebugger
    {
        private readonly ILogger _logger;

        private FluentDebugger(ILogger logger)
        {
            _logger = logger;
        }

        public static FluentDebugger Create(ILogger logger)
        {
            return new FluentDebugger(logger);
        }

        public static FluentDebugger Create()
        {
            return new FluentDebugger(NullLogger.Instance);
        }

        public TResult Run<TResult>(Func<TResult> func)
        {
            var watch = Stopwatch.StartNew();

            var result = func();

            _logger?.LogInformation($"Execution time: {watch.ElapsedMilliseconds}ms");

            return result;
        }

        public async Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func)
        {
            var watch = Stopwatch.StartNew();

            var result = await func();

            _logger?.LogInformation($"Execution time: {watch.ElapsedMilliseconds}ms");

            return result;
        }
    }
}