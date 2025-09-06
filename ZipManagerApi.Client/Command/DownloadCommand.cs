using System;
using System.Runtime.InteropServices.Marshalling;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class DownloadCommand : ICommand
{
    private static async Task SaveFile(Stream stream, string destination)
    {
        try
        {
            await using var fileStream = new FileStream(
                destination,
                FileMode.Create,
                FileAccess.Write
            );
            await stream.CopyToAsync(fileStream);
        }
        catch (FileNotFoundException ex)
        {
            throw new PathException(ex.Message);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new PathException(ex.Message);
        }
    }

    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        var splitted = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (splitted.Length != 2)
            throw new CommandException("Download command requires two args: id pathToDownload");
        var (idStr, path) = (splitted[0], splitted[1]);
        if (!Guid.TryParse(idStr, out var id))
            throw new CommandException($"Failed to parse {idStr} to Guid");
        using var stream = await handler.DownloadArchive(id);
        await SaveFile(stream, path);
        return "Archive saved to " + path;
    }
}
