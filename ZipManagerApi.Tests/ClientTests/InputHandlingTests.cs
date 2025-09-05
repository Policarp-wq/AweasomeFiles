using System;
using ZipManagerApi.Client.Command;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.UserInput;

namespace ZipManagerApi.Tests.ClientTests;

public class InputHandlingTests
{
    [Theory]
    [InlineData("download 3124", UserCommand.Download)]
    [InlineData("list", UserCommand.List)]
    [InlineData("status 1453", UserCommand.Status)]
    [InlineData("zip d3132", UserCommand.Zip)]
    public void CommandsExtract_Correctly(string input, UserCommand expected)
    {
        var extracted = UserInputHandler.ExtractCommandAndArgs(input);
        Assert.Equal(expected, extracted.command);
    }

    [Fact]
    public void Throws_WhenCommand_IsUnknown()
    {
        Assert.Throws<CommandException>(() =>
            UserInputHandler.ExtractCommandAndArgs("unknown_command lol")
        );
    }

    [Fact]
    public void Args_AreSeparated_Correctly()
    {
        string command = "download";
        string args = "214r94k ke9f     effed";
        string input = command + " " + args;
        var (_, extractedArgs) = UserInputHandler.ExtractCommandAndArgs(input);
        Assert.Equal(args, extractedArgs);
    }
}
