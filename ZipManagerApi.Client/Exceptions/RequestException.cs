using System;

namespace ZipManagerApi.Client.Exceptions;

public class RequestException : AppException
{
    public RequestException(string message)
        : base(message) { }
}
