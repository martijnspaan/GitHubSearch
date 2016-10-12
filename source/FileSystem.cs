using System.Collections.Generic;
using System.IO;

namespace GitHubSearch
{
    internal class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteAllText(string filePath, string content)
        {
            DirectoryInfo directory = Directory.GetParent(filePath);
            EnsureDirectoryExists(directory.FullName);

            File.WriteAllText(filePath, content);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public void DeleteDirectory(string path)
        {
            if (DirectoryExists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Delete(true);
            }
        }

        public IEnumerable<string> EnumerateFiles(string directoryPath, string filePattern)
        {
            return DirectoryExists(directoryPath)
                ? Directory.EnumerateFiles(directoryPath, filePattern)
                : new string[0];
        }

        public void RemoveFolder(string directoryPath)
        {
            Directory.Delete(directoryPath, true);
        }
    }
}