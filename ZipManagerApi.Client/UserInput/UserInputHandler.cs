using System;
using ZipManagerApi.Client.Command;
using ZipManagerApi.Client.Exceptions;

namespace ZipManagerApi.Client.UserInput;

public static class UserInputHandler
{
    public static (UserCommand command, string args) ExtractCommandAndArgs(string input)
    {
        var splitted = new List<string>(input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries));
        if (splitted.Count == 1)
            splitted.Add("");
        if (splitted.Count != 2)
            throw new InputException("Wrong input format");
        var commandType = UserCommandConverter.Convert(splitted[0]);
        return (commandType, splitted[1]);
    }
}
