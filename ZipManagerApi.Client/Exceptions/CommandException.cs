using System;

namespace ZipManagerApi.Client.Exceptions;

public class CommandException : AppException
{
    public CommandException(string message)
        : base(message) { }
}
