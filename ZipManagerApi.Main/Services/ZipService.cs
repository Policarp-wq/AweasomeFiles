using System;
using ZipManagerApi.Domain;
using ZipManagerApi.Main.Abstractions;

namespace ZipManagerApi.Main.Services;

public class ZipService : IZipService
{
    private readonly ZipMaster _zipMaster;
    private readonly ILogger<ZipService> _logger;

    public ZipService(string rootPath, ILogger<ZipService> logger)
    {
        _zipMaster = new ZipMaster(rootPath);
        _logger = logger;
    }

    public List<string> GetAllFiles()
    {
        _logger.LogInformation("All files requested");
        return _zipMaster.GetAllFiles();
    }

    public FileStream? GetArchive(Guid id)
    {
        _logger.LogInformation("Archive download requested for id {Id}", id);
        return _zipMaster.GetArchivedFileStream(id);
    }

    public ZipStatus? GetProgress(Guid id)
    {
        _logger.LogInformation("Progress requested for process with id {Id}", id);
        return _zipMaster.GetStatus(id);
    }

    public Guid ZipFiles(List<string> fileNames)
    {
        _logger.LogInformation("Zip task requested for {Count} files", fileNames);
        var task = _zipMaster.GetZipTask(fileNames, CancellationToken.None);
        task.ZipTask.Start();
        _logger.LogInformation(
            "Zip task with zip id {Id} started. Executing task id {TaskId}",
            task.ProcessId,
            task.ZipTask.Id
        );
        return task.ProcessId;
    }
}
