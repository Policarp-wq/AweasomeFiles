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

    public Progress? GetProgress(Guid id)
    {
        if (_processingIds.Contains(id))
            return Progress.Processing;
        if (File.Exists(GetArchiveFilePath(id)))
            return Progress.Finished;
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

    public async Task<string> ZipFiles(List<string> fileNames, CancellationToken token)
    {
        var filePaths = GetFullFilePaths(fileNames);

        Guid processId = Guid.CreateVersion7();
        string outputFileName = GetZippedName(processId.ToString());
        string outputPath = GetFullOutputPath(outputFileName);

        using var zipFile = new FileStream(outputPath, FileMode.Create);
        using var archive = new ZipArchive(zipFile, ZipArchiveMode.Create);
        try
        {
            foreach (var file in filePaths)
            {
                token.ThrowIfCancellationRequested();
                string entryName = Path.GetFileName(file);
                await Task.Run(
                    () => archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal),
                    token
                );
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
    }

    public FileStream GetArchivedFile(Guid fileId)
    {
        var path = GetArchiveFilePath(fileId);
        if (!File.Exists(path))
            throw new ZipMasterException($"No zip with id {fileId}");
        var stream = new FileStream(path, FileMode.Open);
        return stream;
    }
}
