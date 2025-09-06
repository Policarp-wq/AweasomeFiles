using ZipManagerApi.Client.Command;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;
using ZipManagerApi.Client.UserInput;

void PrintHelp()
{
    System.Console.WriteLine("Available commands:");
    foreach (var com in UserCommandConverter.AvailableCommandNames)
    {
        Console.WriteLine(com);
    }
    Console.WriteLine("Print `help` for this message");
}

if (args.Length == 0)
{
    Console.WriteLine("You need to specify url link in args");
    return -1;
}
string zipManagerApiLink = args[0];

var requestHandler = new RequestHandler(new HttpClient(), zipManagerApiLink);
Console.WriteLine("Starting client for zipping files.");
PrintHelp();
CancellationTokenSource source = new();
Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Stopping program...");
    source.Cancel();
    e.Cancel = false;
};
Console.WriteLine("Waiting for input.\nPress <Ctrl + C> to exit...");
try
{
    while (!source.IsCancellationRequested)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            continue;
        if (input == "help")
        {
            PrintHelp();
            continue;
        }
        try
        {
            var (command, arg) = UserInputHandler.ExtractCommandAndArgs(input);
            var output = await CommandExecuter.ExecuteCommand(requestHandler, command, arg);
            Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
catch (OperationCanceledException) { }
return 0;
