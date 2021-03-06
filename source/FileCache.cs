﻿using System;

namespace GitHubSearch
{
    /// <summary>
    /// Caches downloaded configurations in local cache folder.
    /// </summary>
    internal class FileCache : IFileCache
    {
        private readonly IFileSystem fileSystem;
        private static readonly string LocalCacheFolder =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\GitHubSearch\\Cache";

        public FileCache(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            fileSystem.EnsureDirectoryExists(LocalCacheFolder);
        }

        public string GetCachedFileContent(long repositoryId, string repositoryName, string filePath, string sha, Func<long, string, string> loadContent)
        {
            string cacheKey = CreateCacheKey(filePath, sha);

            string cachedFilePath = $"{LocalCacheFolder}\\{repositoryName}\\{cacheKey}.cache";

            if (fileSystem.FileExists(cachedFilePath))
            {
                return fileSystem.ReadAllText(cachedFilePath);
            }

            string actualContent = loadContent(repositoryId, filePath);

            ClearCacheFor(repositoryName, filePath);

            StoreInCache(cachedFilePath, actualContent);

            return actualContent;
        }

        private void StoreInCache(string cachedFilePath, string actualContent)
        {
            fileSystem.WriteAllText(cachedFilePath, actualContent);
        }

        private string CreateCacheKey(string filePath, string sha)
        {
            string fileKey = filePath.Replace("/", "-");
            return $"{fileKey}-{sha}";
        }

        private void ClearCacheFor(string repositoryName, string filePath)
        {
            string directoryPath = $"{LocalCacheFolder}\\{repositoryName}";
            string fileKey = filePath.Replace("/", "-");

            foreach (string path in fileSystem.EnumerateFiles(directoryPath, $"{fileKey}*.*"))
            {
                fileSystem.DeleteFile(path);
            }
        }

        public void Flush()
        {
            fileSystem.RemoveFolder(LocalCacheFolder);
        }
    }

    internal interface IFileCache
    {
        /// <summary>
        /// Removes all cached content from the cache folder.
        /// </summary>
        void Flush();
        
        /// <summary>
        /// Loads the search hit from local cache folder when available. Reverts to supplied loadContent function when not found in cache.
        /// </summary>
        string GetCachedFileContent(long repositoryId, string repositoryName, string filePath, string sha, Func<long, string, string> loadContent);
    }
}