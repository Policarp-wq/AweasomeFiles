using System;

namespace ZipManagerApi.Client.RequestHandler;

public interface IRequestHandler
{
    Task<Stream> DownloadArchive(Guid id);
    Task<List<string>> GetFiles();
    Task<string> GetStatus(Guid id);
    Task<Guid> ZipFiles(List<string> fileNames);
}
