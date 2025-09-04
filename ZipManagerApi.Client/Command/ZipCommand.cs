using System;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class ZipCommand : ICommand
{
    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        List<string> files = new(args.Split(' '));
        return "Zip process id started with id: " + (await handler.ZipFiles(files)).ToString();
    }
}
