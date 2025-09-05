using System;
using System.Threading.Tasks;
using ZipManagerApi.Domain;
using ZipManagerApi.Domain.Exceptions;

namespace ZipManagerApi.Tests.ZipMasterTests;

public class ZipProcessTests : IClassFixture<FileFixture>
{
    private readonly ZipMaster _zipMaster;

    public ZipProcessTests(FileFixture files)
    {
        _zipMaster = new(files.Init("_process", 10));
    }

    private bool DoesZipExist(Guid id) => File.Exists(_zipMaster.GetArchiveFilePath(id));

    [Fact]
    public void ZipMasterFails_WhenNoFilesProvided()
    {
        Assert.Throws<ZipMasterException>(() => _zipMaster.GetZipTask([], CancellationToken.None));
    }

    [Fact]
    public void ZipMasterFails_WhenNoFile()
    {
        Assert.Throws<ZipMasterException>(() =>
            _zipMaster.GetZipTask(["lol.kek"], CancellationToken.None)
        );
    }

    [Fact]
    public void ZipMasterFails_WhenPartOfTheFiles_Incorrect()
    {
        Assert.Throws<ZipMasterException>(() =>
            _zipMaster.GetZipTask(["1", "2", "lol.kek"], CancellationToken.None)
        );
    }

    [Fact]
    public async Task ZipMaster_ProvidesFinishedStatus_WhenFinished()
    {
        var process = _zipMaster.GetZipTask(["1", "2", "3"], CancellationToken.None);
        process.ZipTask.Start();
        await process.ZipTask;
        Assert.Equal(ZipStatus.Finished, _zipMaster.GetStatus(process.ProcessId));
    }

    [Fact]
    public async Task ZipMaster_CreatesZip_WhenFinished()
    {
        var process = _zipMaster.GetZipTask(["1", "2", "3"], CancellationToken.None);
        process.ZipTask.Start();
        await process.ZipTask;
        Assert.Equal(ZipStatus.Finished, _zipMaster.GetStatus(process.ProcessId));
        Assert.True(DoesZipExist(process.ProcessId));
    }

    [Fact]
    public async Task NoFileCreated_WhenZipping_Failed()
    {
        CancellationTokenSource source = new();
        var process = _zipMaster.GetZipTask(["1"], source.Token);
        source.Cancel();
        process.ZipTask.Start();
        await Assert.ThrowsAsync<ZipMasterException>(async () => await process.ZipTask);
        Assert.Null(_zipMaster.GetArchivedFileStream(process.ProcessId));
        Assert.False(DoesZipExist(process.ProcessId));
    }

    [Fact]
    public void ZipMaster_ProvidesNullStatus_WhenZip_DidNotStart()
    {
        var process = _zipMaster.GetZipTask(["1", "2", "3"], CancellationToken.None);
        Assert.Null(_zipMaster.GetStatus(process.ProcessId));
    }

    [Fact]
    public void NoFileStream_WhenWrongId()
    {
        Assert.Null(_zipMaster.GetArchivedFileStream(Guid.CreateVersion7()));
    }
}
