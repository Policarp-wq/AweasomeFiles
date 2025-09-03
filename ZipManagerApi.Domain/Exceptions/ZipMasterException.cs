using System;

namespace ZipManagerApi.Domain.Exceptions;

public class ZipMasterException : AppException
{
    public ZipMasterException(string message)
        : base(message) { }
}
