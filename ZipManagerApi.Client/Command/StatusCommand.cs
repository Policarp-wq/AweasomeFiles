using System;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class StatusCommand : ICommand
{
    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        if (!Guid.TryParse(args, out var id))
            throw new CommandException($"Failed to parse {args} to Guid");
        var status = await handler.GetStatus(id);
        return $"Process with id {id}: " + status;
    }
}
