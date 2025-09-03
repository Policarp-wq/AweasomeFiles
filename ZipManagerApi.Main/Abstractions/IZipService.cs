using System;
using ZipManagerApi.Domain;

namespace ZipManagerApi.Main.Abstractions;

public interface IZipService
{
    FileStream? GetArchive(Guid id);

    Guid ZipFiles(List<string> fileNames);
    List<string> GetAllFiles();
    ZipProgress? GetProgress(Guid id);
}
