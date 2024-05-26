using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentDebug.Loggers;
using FluentDebug.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluentDebug
{
    public class FluentDebugger
    {
        private readonly ILogAdapter _logger;
        private bool _logParameters;
        private Func<ExceptionContext, Exception> _rethrowHandler;

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

        public FluentDebugger LogParameters()
        {
            _logParameters = true;
            return this;
        }

        public FluentDebugger Rethrow(Func<ExceptionContext, Exception> rethrowHandler)
        {
            _rethrowHandler = rethrowHandler;

            return this;
        }

        public TResult Run<TResult>(Expression<Func<TResult>> expression)
        {
            var watch = Stopwatch.StartNew();
            var callerInfoModel = new CallerInfoModel<TResult>(expression);

            try
            {
                var result = expression.Compile().Invoke();
                WriteLog(watch, callerInfoModel);

                return result;
            }
            catch (Exception e)
            {
                if (_rethrowHandler != null)
                {
                    throw _rethrowHandler.Invoke(new ExceptionContext
                    {
                        Exception = e,
                        MethodName = callerInfoModel.MethodName,
                        Parameters = callerInfoModel.Parameters,
                        ElapsedMilliseconds = watch.ElapsedMilliseconds
                    });
                }

                throw;
            }
        }

        public async Task<TResult> RunAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            var watch = Stopwatch.StartNew();
            var callerInfoModel = new CallerInfoModel<Task<TResult>>(expression);

            try
            {
                var result = await expression.Compile().Invoke();
                WriteLog(watch, callerInfoModel);

                return result;
            }
            catch (Exception e)
            {
                if (_rethrowHandler != null)
                {
                    throw _rethrowHandler.Invoke(new ExceptionContext
                    {
                        Exception = e,
                        MethodName = callerInfoModel.MethodName,
                        Parameters = callerInfoModel.Parameters,
                        ElapsedMilliseconds = watch.ElapsedMilliseconds
                    });
                }

                throw;
            }
        }

        private void WriteLog<TResult>(Stopwatch watch, CallerInfoModel<TResult> callerInfoModel)
        {
            if (!callerInfoModel.IsMethodCall)
            {
                _logger.Log($"Execution time: {watch.ElapsedMilliseconds}ms");
                return;
            }

            var message = callerInfoModel.Parameters.Count == 0
                ? $"[{callerInfoModel.MethodName}()] Execution time: {watch.ElapsedMilliseconds}ms"
                : $"[{callerInfoModel.MethodName}()] Parameters: `{callerInfoModel.FormatParameters()}` | Execution time: {watch.ElapsedMilliseconds}ms";
            _logger.Log(message);
        }
    }
}