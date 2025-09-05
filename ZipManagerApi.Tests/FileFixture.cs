using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;

namespace ZipManagerApi.Tests;

public class FileFixture : IDisposable
{
    private string FilesDirPath = Path.Combine(Path.GetTempPath(), "zip_tests");

    public FileFixture() { }

    public string Init(string postfix, int cnt)
    {
        FilesDirPath += postfix;
        Directory.CreateDirectory(FilesDirPath);
        FillDirectory(cnt);
        return FilesDirPath;
    }

    public void FillDirectory(int cnt)
    {
        if (!Directory.Exists(FilesDirPath))
            throw new DirectoryNotFoundException(FilesDirPath);
        for (int i = 0; i < cnt; ++i)
        {
            string fileName = Path.Combine(FilesDirPath, (i + 1).ToString());
            using var file = new FileStream(fileName, FileMode.Create);
        }
    }

    public void Dispose()
    {
        Directory.Delete(FilesDirPath, true);
    }
}
