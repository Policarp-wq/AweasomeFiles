using System;

namespace ZipManagerApi.Client.RequestHandler;

public interface IRequestHandler
{
    Task DownloadArchive(Guid id, string destination);
    Task<List<string>> GetFiles();
    Task<string> GetStatus(Guid id);
    Task<Guid> ZipFiles(List<string> fileNames);
}
