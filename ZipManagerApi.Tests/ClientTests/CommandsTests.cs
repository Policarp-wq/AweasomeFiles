using System;
using System.Threading.Tasks;
using Moq;
using ZipManagerApi.Client.Command;
using ZipManagerApi.Client.Exceptions;
using ZipManagerApi.Client.RequestHandler;

namespace ZipManagerApi.Tests.ClientTests;

public class CommandsTests
{
    [Theory]
    [InlineData("not_guid not_path")]
    [InlineData("01991b4a-b2e3-7865-acee-2e5d97a012d4")]
    [InlineData("/tmp/output.zip")]
    public async Task DownloadCommand_Throws_WhenInvalidArgs(string wrongArgs)
    {
        var mock = new Mock<IRequestHandler>(MockBehavior.Default);
        await Assert.ThrowsAsync<CommandException>(async () =>
            await new DownloadCommand().Execute(mock.Object, wrongArgs)
        );
    }

    [Fact]
    public async Task DownloadCommand_ThrowsException_WhenInvalidPath()
    {
        var mock = new Mock<IRequestHandler>(MockBehavior.Default);
        string wrongArgs = "01991b4a-b2e3-7865-acee-2e5d97a012d4 /not_a/path/lol.zip";
        await Assert.ThrowsAsync<PathException>(async () =>
            await new DownloadCommand().Execute(mock.Object, wrongArgs)
        );
    }

    [Fact]
    public async Task DownloadCommand_SavesFile()
    {
        var mock = new Mock<IRequestHandler>(MockBehavior.Default);
        Guid guid = Guid.Parse("01991b4a-b2e3-7865-acee-2e5d97a012d4");
        string filePath = Path.Combine(Path.GetTempPath(), guid.ToString());
        string rightArgs = $"{guid} {filePath}";
        mock.Setup(r => r.DownloadArchive(guid))
            .Returns(Task.FromResult<Stream>(new MemoryStream(1)));
        await new DownloadCommand().Execute(mock.Object, rightArgs);
        Assert.True(File.Exists(filePath));
        File.Delete(filePath);
    }
}
