using System;
using System.Collections.Generic;

namespace FluentDebug.Models;

public class ExceptionContext
{
    public Exception Exception { get; set; }
    public string MethodName { get; set; }
    public List<KeyValuePair<string, object>> Parameters { get; set; }
    public long ElapsedMilliseconds { get; set; }
}