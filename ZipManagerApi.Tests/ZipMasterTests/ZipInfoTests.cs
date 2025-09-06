using System;
using System.IO.Compression;
using System.Threading.Tasks;
using ZipManagerApi.Client.Command;
using ZipManagerApi.Domain;

namespace ZipManagerApi.Tests.ZipMasterTests;

public class ZipInfoTests : IClassFixture<FileFixture>
{
    private readonly ZipMaster _zipMaster;

    public ZipInfoTests(FileFixture files)
    {
        _zipMaster = new(files.Init("_info", 10));
    }

    private bool DoesZipExist(Guid id) => File.Exists(_zipMaster.GetArchiveFilePath(id));

    [Fact]
    public void ZipMaster_ProvidesFile_Without_OutputDir()
    {
        Assert.DoesNotContain(_zipMaster.OutputSubdirName, _zipMaster.GetAllFiles());
    }

    [Fact]
    public async Task AllFiles_CanBe_Zipped()
    {
        var process = _zipMaster.GetZipTask(_zipMaster.GetAllFiles(), CancellationToken.None);
        process.ZipTask.Start();
        await process.ZipTask;
        using var stream = _zipMaster.GetArchivedFileStream(process.ProcessId);
        Assert.NotNull(stream);
        using var archive = ZipFile.OpenRead(_zipMaster.GetArchiveFilePath(process.ProcessId));
        Assert.Equal(_zipMaster.GetAllFiles().Count, archive.Entries.Count);
    }

    [Fact]
    public async Task RepeatedFiles_CanBe_Zipped()
    {
        List<string> repeatedFiles = ["1", "1", "2"];
        var process = _zipMaster.GetZipTask(repeatedFiles, CancellationToken.None);
        process.ZipTask.Start();
        await process.ZipTask;
        using var stream = _zipMaster.GetArchivedFileStream(process.ProcessId);
        Assert.NotNull(stream);
        using var archive = ZipFile.OpenRead(_zipMaster.GetArchiveFilePath(process.ProcessId));
        Assert.Equal(repeatedFiles.Count, archive.Entries.Count);
    }
}
