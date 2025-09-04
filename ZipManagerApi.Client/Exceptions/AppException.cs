using System;

namespace ZipManagerApi.Client.Exceptions;

public class AppException : Exception
{
    public AppException(string message)
        : base(message) { }
}
