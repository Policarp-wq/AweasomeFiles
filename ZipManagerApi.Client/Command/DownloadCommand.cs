using System;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class DownloadCommand : ICommand
{
    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        var splitted = args.Split(' ');
        if (splitted.Length != 2)
            throw new CommandException("Download command requires two args: id pathToDownload");
        var (idStr, path) = (splitted[0], splitted[1]);
        if (!Guid.TryParse(idStr, out var id))
            throw new CommandException($"Failed to parse {idStr} to Guid");
        await handler.DownloadArchive(id, path);
        return "Download completed to " + path;
    }
}
