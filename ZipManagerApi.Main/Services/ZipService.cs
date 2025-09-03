using System;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;

namespace ZipManagerApi.Main.Services;

public class ZipService : IZipService
{
    private readonly ZipMaster _zipMaster;

    public ZipService(string rootPath)
    {
        _zipMaster = new ZipMaster(rootPath);
    }

    public List<string> GetAllFiles()
    {
        return _zipMaster.GetAllFiles();
    }

    public FileStream? GetArchive(Guid id)
    {
        return _zipMaster.GetArchivedFile(id);
    }

    public ZipProgress? GetProgress(Guid id)
    {
        return _zipMaster.GetProgress(id);
    }

    public Guid ZipFiles(List<string> fileNames)
    {
        return _zipMaster.StartZippingFiles(fileNames, CancellationToken.None);
    }
}
