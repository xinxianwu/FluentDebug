using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
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

        public TResult Run<TResult>(Expression<Func<TResult>> expression)
        {
            var watch = Stopwatch.StartNew();

            MethodInfo methodInfo = null;
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                methodInfo = methodCallExpression.Method;
            }

            var result = expression.Compile().Invoke();

            LofMethod(watch, methodInfo);

            return result;
        }

        public async Task<TResult> RunAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            var watch = Stopwatch.StartNew();
            
            MethodInfo methodInfo = null;
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                methodInfo = methodCallExpression.Method;
            }

            var result = await expression.Compile().Invoke();

            LofMethod(watch, methodInfo);

            return result;
        }

        private void LofMethod(Stopwatch watch, MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                _logger.LogInformation($"Execution time: {watch.ElapsedMilliseconds}ms");
            }
            else
            {
                _logger.LogInformation($"[{methodInfo.Name}()] Execution time: {watch.ElapsedMilliseconds}ms");
            }
            
        }
    }
}