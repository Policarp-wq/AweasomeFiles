using System;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class ListCommand : ICommand
{
    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        List<string> res = await handler.GetFiles();
        return "Available files: " + String.Join(" ", res);
    }
}
