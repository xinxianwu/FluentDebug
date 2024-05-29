namespace FluentDebug.Tests.Exceptions;

public class MyException : Exception
{
    public MyException(Exception exception, string message) : base(message, exception)
    {
    }
}