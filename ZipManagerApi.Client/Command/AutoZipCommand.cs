using System;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public class AutoZipCommand : ICommand
{
    public const int REQUEST_INTERVAL = 1;
    public const int MAX_TRIES = 20;
    public const string FINISH_STATUS = "Finished";

    public async Task<string> Execute(IRequestHandler handler, string args)
    {
        List<string> splittedArgs = new(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (splittedArgs.Count < 2)
            throw new CommandException(
                "Auto command requires at least 2 args: files and result file path"
            );
        List<string> files = splittedArgs.GetRange(0, splittedArgs.Count - 1);
        string destination = splittedArgs[splittedArgs.Count - 1];
        var id = await handler.ZipFiles(files);
        Console.WriteLine($"Zipping process started with id {id}");
        for (int i = 0; i < MAX_TRIES; ++i)
        {
            await Task.Delay(REQUEST_INTERVAL * 1000);
            var status = await handler.GetStatus(id);
            Console.WriteLine($"Try 1: {status}");
            if (status == FINISH_STATUS)
                break;
        }
        Console.WriteLine($"Downloading archive with id {id}");
        return await new DownloadCommand().Execute(handler, $"{id} {destination}");
    }
}
