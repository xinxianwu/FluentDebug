using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentDebug.Models;

public class CallerInfoModel<TResult>
{
    private readonly bool _isMethodCallExpression;
    private readonly Expression<Func<TResult>> _expression;
    private readonly Lazy<List<KeyValuePair<string, object>>> _parameters;
    private readonly MethodCallExpression _methodCallExpression;

    public bool IsMethodCall => _expression.Body is MethodCallExpression;

    public string MethodName => _isMethodCallExpression ? _methodCallExpression.Method.Name : string.Empty;
    public List<KeyValuePair<string, object>> Parameters => _parameters.Value;

    public CallerInfoModel(Expression<Func<TResult>> expression)
    {
        _expression = expression;
        _isMethodCallExpression = _expression.Body is MethodCallExpression;
        _methodCallExpression = _expression.Body as MethodCallExpression;
        _parameters = new Lazy<List<KeyValuePair<string, object>>>(GetParameters);
    }

    public string FormatParameters()
    {
        var parameterString = Parameters
            .Select(x =>
            {
                var valueString = x.Value switch
                {
                    IDictionary dictionary => FormatDictionary(dictionary),
                    IEnumerable enumerable => FormatEnumerable(enumerable),
                    _ => x.Value.ToString()
                };

                return $"{x.Key}: {valueString}";
            })
            .ToList();
        return string.Join(", ", parameterString);
    }

    private List<KeyValuePair<string, object>> GetParameters()
    {
        if (!_isMethodCallExpression)
        {
            return new List<KeyValuePair<string, object>>();
        }

        var parameterInfos = _methodCallExpression.Method.GetParameters();
        var arguments = _methodCallExpression.Arguments.ToList();

        return parameterInfos
            .Zip(arguments, (parameterInfo, argumentExpression) =>
            {
                var value = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
                return new KeyValuePair<string, object>(parameterInfo.Name, value);
            })
            .ToList();
    }

    private string FormatDictionary(IDictionary dictionary)
    {
        var values = dictionary.Keys.Cast<object>().Select(key => $"{key}: {dictionary[key]}");
        return $"{{{string.Join(", ", values)}}}";
    }

    private string FormatEnumerable(IEnumerable enumerable)
    {
        var values = enumerable.Cast<object>().Select(x => x.ToString());
        return $"[{string.Join(", ", values)}]";
    }
}