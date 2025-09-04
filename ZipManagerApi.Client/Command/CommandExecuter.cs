using System;
using System.Threading.Tasks;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Client.Command;

public static class CommandExecuter
{
    private static readonly Dictionary<UserCommand, ICommand> _commands = new()
    {
        { UserCommand.Zip, new ZipCommand() },
        { UserCommand.List, new ListCommand() },
        { UserCommand.Status, new StatusCommand() },
        { UserCommand.Download, new DownloadCommand() },
    };

    public static async Task<string> ExecuteCommand(
        IRequestHandler handler,
        UserCommand command,
        string args
    )
    {
        if (!_commands.TryGetValue(command, out var executor))
            throw new CommandException($"Command {Enum.GetName(command)} is unhandled");
        return await executor.Execute(handler, args);
    }
}
