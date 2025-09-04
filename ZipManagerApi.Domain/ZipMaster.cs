using System;
using System.Diagnostics;
using System.IO.Compression;
using ZipManagerApi.Domain.Exceptions;

namespace ZipManagerApi.Domain;

public class ZipMaster
{
    public readonly string RootFolderPath;
    public const string OutputSubdirName = "zip_output";
    private HashSet<Guid> _processingIds;

    public ZipMaster(string rootFolderPath)
    {
        RootFolderPath = rootFolderPath;
        if (!Directory.Exists(rootFolderPath))
        {
            throw new ZipMasterException($"Provided path {rootFolderPath} does not exist");
        }
        Directory.CreateDirectory(Path.Combine(RootFolderPath, OutputSubdirName));
        _processingIds = [];
    }

    public List<string> GetAllFiles()
    {
        return Directory
            .GetFiles(RootFolderPath)
            .Select(Path.GetFileName)
            .Where(name => name != null)
            .ToList()!;
    }

    public ZipStatus? GetProgress(Guid id)
    {
        if (_processingIds.Contains(id))
            return ZipStatus.Processing;
        if (File.Exists(GetArchiveFilePath(id)))
            return ZipStatus.Finished;
        return null;
    }

    private static string GetZippedName(string fileName) => $"{fileName}.zip";

    private string GetArchiveFilePath(Guid fileId) =>
        GetFullOutputPath(GetZippedName(fileId.ToString()));

    private string GetFullOutputPath(string zipFileName) =>
        Path.Combine(RootFolderPath, OutputSubdirName, zipFileName);

    private string GetFullFileEntryPath(string file) => Path.Combine(RootFolderPath, file);

    //TODO: lazy with IEnumerable
    private List<string> GetFullFilePaths(List<string> fileNames)
    {
        List<string> res = [];
        foreach (var name in fileNames)
        {
            var full = GetFullFileEntryPath(name);
            if (!File.Exists(full))
                throw new ZipMasterException($"No file found with name {name}");
            res.Add(full);
        }
        return res;
    }

    public async Task<string> ZipFiles(
        Guid processId,
        List<string> filePaths,
        CancellationToken token
    )
    {
        string outputFileName = GetZippedName(processId.ToString());
        string outputPath = GetFullOutputPath(outputFileName);

        using var zipFile = new FileStream(outputPath, FileMode.Create);
        using var archive = new ZipArchive(zipFile, ZipArchiveMode.Create);
        try
        {
            _processingIds.Add(processId);
            foreach (var file in filePaths)
            {
                token.ThrowIfCancellationRequested();
                string entryName = Path.GetFileName(file);
                await Task.Run(
                    () => archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal),
                    token
                );
                // For status testing
                // await Task.Delay(5000);
            }
            return zipFile.Name;
        }
        catch (OperationCanceledException)
        {
            zipFile.Close();
            zipFile.Dispose();
            File.Delete(outputPath);
            throw new ZipMasterException("Zip process cancelled");
        }
        finally
        {
            _processingIds.Remove(processId);
        }
    }

    public Guid StartZippingFiles(List<string> fileNames, CancellationToken token)
    {
        var filePaths = GetFullFilePaths(fileNames);
        Guid processId = Guid.CreateVersion7();
        Task.Run(async () => await ZipFiles(processId, filePaths, token));
        return processId;
    }

    public FileStream? GetArchivedFile(Guid fileId)
    {
        var path = GetArchiveFilePath(fileId);
        if (_processingIds.Contains(fileId))
            throw new ZipMasterException(
                "Attempted to get archived file that has not been completed"
            );
        if (!File.Exists(path))
            return null;
        var stream = new FileStream(path, FileMode.Open);
        return stream;
    }
}
