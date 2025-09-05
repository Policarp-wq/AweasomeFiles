using System;

namespace ZipManagerApi.Client.Exceptions;

public class PathException : AppException
{
    public PathException(string message)
        : base(message) { }
}
