using System;
using System.IO;

namespace GitHubSearch
{
    /// <summary>
    /// Caches downloaded configurations in local cache folder.
    /// </summary>
    internal class FileCache : IFileCache
    {
        private const string LocalCacheFolder = ".\\Cache";

        public FileCache()
        {
            if (!Directory.Exists(LocalCacheFolder))
            {
                Directory.CreateDirectory(LocalCacheFolder);
            }
        }
                
        /// <summary>
        /// Loads the search hit from local cache folder when available. Reverts to supplied loadContent function when not found in cache.
        /// </summary>
        public string GetCachedFileContent(int repositoryId, string filePath, string sha, Func<int, string, string> loadContent)
        {
            var cacheKey = CreateCacheKey(repositoryId, filePath, sha);

            var cachedFilePath = $"{LocalCacheFolder}\\{cacheKey}.cache";

            if (File.Exists(cachedFilePath))
            {
                return File.ReadAllText(cachedFilePath);
            }

            var actualContent = loadContent(repositoryId, filePath);

            ClearCacheFor(repositoryId, filePath);

            StoreInCache(cachedFilePath, actualContent);

            return actualContent;
        }

        private static void StoreInCache(string cachedFilePath, string actualContent)
        {
            File.WriteAllText(cachedFilePath, actualContent);
        }

        private string CreateCacheKey(int repositoryId, string filePath, string sha)
        {
            var fileKey = filePath.Replace("/", "-");
            return $"{repositoryId}-{fileKey}-{sha}";
        }

        private static void ClearCacheFor(int repositoryId, string filePath)
        {
            var fileKey = filePath.Replace("/", "-");
            foreach (var path in Directory.EnumerateFiles(LocalCacheFolder, $"{repositoryId}-{fileKey}*.*"))
            {
                File.Delete(path);
            }
        }
    }

    internal interface IFileCache
    {
        string GetCachedFileContent(int repositoryId, string filePath, string sha, Func<int, string, string> loadContent);
    }
}
