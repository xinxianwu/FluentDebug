using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluentDebug
{
    public class FluentDebug
    {
        private readonly ILogger _logger;

        private FluentDebug(ILogger logger)
        {
            _logger = logger;
        }

        public static FluentDebug Create(ILogger logger)
        {
            return new FluentDebug(logger);
        }

        public static FluentDebug Create()
        {
            return new FluentDebug(NullLogger.Instance);
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