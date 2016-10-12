using System.Collections.Generic;
using System.IO;

namespace GitHubSearch
{
    internal interface IFileSystem
    {
        bool DirectoryExists(string path);

        DirectoryInfo CreateDirectory(string path);

        void EnsureDirectoryExists(string path);

        bool FileExists(string path);

        string ReadAllText(string filePath);

        void WriteAllText(string filePath, string content);

        void DeleteFile(string filePath);

        void DeleteDirectory(string path);

        IEnumerable<string> EnumerateFiles(string directoryPath, string filePattern);

        void RemoveFolder(string cacheDirectory);
    }
}