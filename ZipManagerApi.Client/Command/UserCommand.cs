using System;
using ZipManagerApi.Client.Exceptions;

namespace ZipManagerApi.Client.Command;

public enum UserCommand
{
    List,
    Zip,
    Status,
    Download,
}

public static class UserCommandConverter
{
    public static readonly string[] AvailableCommandNames =
    [
        "list - get available files. ex: > list",
        "zip - zip certain files. ex: > zip file1 file2 ...",
        "status - get process status by id. ex: > status 1e809-101",
        "download - download zipped file by id. ex: > download 1e809-101",
    ];

    public static UserCommand Convert(string command)
    {
        return command switch
        {
            "list" => UserCommand.List,
            "zip" => UserCommand.Zip,
            "status" => UserCommand.Status,
            "download" => UserCommand.Download,
            _ => throw new CommandException("Failed to convert user command"),
        };
    }
}
