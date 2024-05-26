using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentDebug.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluentDebug
{
    public class FluentDebugger
    {
        private readonly ILogAdapter _logger;
        private bool _logParameters;

        private FluentDebugger(ILogAdapter logger)
        {
            _logger = logger;
        }

        public static FluentDebugger Create(ILogAdapter logger)
        {
            return new FluentDebugger(logger);
        }

        public static FluentDebugger Create()
        {
            return new FluentDebugger(new LogAdapter(NullLogger.Instance));
        }

        public TResult Run<TResult>(Expression<Func<TResult>> expression)
        {
            var watch = Stopwatch.StartNew();

            MethodInfo methodInfo = null;
            var parameters = string.Empty;
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                methodInfo = methodCallExpression.Method;
                parameters = _logParameters ? FormatParametersAndArguments(methodInfo, methodCallExpression) : string.Empty;
            }

            var result = expression.Compile().Invoke();

            WriteLog(watch, methodInfo, parameters);

            return result;
        }

        public async Task<TResult> RunAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            var watch = Stopwatch.StartNew();

            MethodInfo methodInfo = null;
            var parameters = string.Empty;
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                methodInfo = methodCallExpression.Method;
                parameters = _logParameters ? FormatParametersAndArguments(methodInfo, methodCallExpression) : string.Empty;
            }

            var result = await expression.Compile().Invoke();

            WriteLog(watch, methodInfo, parameters);

            return result;
        }

        public FluentDebugger LogParameters()
        {
            _logParameters = true;
            return this;
        }

        private static string FormatParametersAndArguments(MethodInfo methodInfo, MethodCallExpression methodCallExpression)
        {
            var parameterInfos = methodInfo.GetParameters();
            var arguments = methodCallExpression.Arguments.ToList();

            var format = parameterInfos
                .Zip(arguments, (parameterInfo, argumentExpression) =>
                {
                    var value = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
                    var valueString = value switch
                    {
                        IDictionary dictionary => FormatDictionary(dictionary),
                        IEnumerable enumerable => FormatEnumerable(enumerable),
                        _ => value.ToString()
                    };

                    return $"{parameterInfo.Name}: {valueString}";
                });
            
            return string.Join(", ", format);
        }

        private static string FormatDictionary(IDictionary dictionary)
        {
            var values = dictionary.Keys.Cast<object>().Select(key => $"{key}: {dictionary[key]}");
            return $"{{{string.Join(", ", values)}}}";
        }

        private static string FormatEnumerable(IEnumerable enumerable)
        {
            var values = enumerable.Cast<object>().Select(x => x.ToString());
            return $"[{string.Join(", ", values)}]";
        }

        private void WriteLog(Stopwatch watch, MethodInfo methodInfo, string parameters)
        {
            if (methodInfo == null)
            {
                _logger.Log($"Execution time: {watch.ElapsedMilliseconds}ms");
            }
            else
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    _logger.Log($"[{methodInfo.Name}()] Execution time: {watch.ElapsedMilliseconds}ms");
                }
                else
                {
                    _logger.Log($"[{methodInfo.Name}()] Parameters: `{parameters}` | Execution time: {watch.ElapsedMilliseconds}ms");
                }
            }
        }
    }
}