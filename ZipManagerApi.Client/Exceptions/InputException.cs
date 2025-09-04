using System;

namespace ZipManagerApi.Client.Exceptions;

public class InputException : AppException
{
    public InputException(string message)
        : base(message) { }
}
