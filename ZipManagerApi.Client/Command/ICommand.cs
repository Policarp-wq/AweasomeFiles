using System;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public interface ICommand
{
    Task<string> Execute(IRequestHandler handler, string args);
}
