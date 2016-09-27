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
        public string GetCachedFileContent(int repositoryId, string repositoryName, string filePath, string sha, Func<int, string, string> loadContent)
        {
            var cacheKey = CreateCacheKey(filePath, sha);

            var cachedFilePath = $"{LocalCacheFolder}\\{repositoryName}\\{cacheKey}.cache";

            if (File.Exists(cachedFilePath))
            {
                return File.ReadAllText(cachedFilePath);
            }

            string actualContent = loadContent(repositoryId, filePath);

            ClearCacheFor(repositoryName, filePath);

            StoreInCache(cachedFilePath, actualContent);

            return actualContent;
        }

        private static void StoreInCache(string cachedFilePath, string actualContent)
        {
            DirectoryInfo folder = Directory.GetParent(cachedFilePath);

            if (!Directory.Exists(folder.FullName))
            {
                Directory.CreateDirectory(folder.FullName);
            }

            File.WriteAllText(cachedFilePath, actualContent);
        }

        private string CreateCacheKey(string filePath, string sha)
        {
            var fileKey = filePath.Replace("/", "-");
            return $"{fileKey}-{sha}";
        }

        private static void ClearCacheFor(string repositoryName, string filePath)
        {
            string directoryPath = $"{LocalCacheFolder}\\{repositoryName}";
            if (Directory.Exists(directoryPath))
            {
                var fileKey = filePath.Replace("/", "-");
                foreach (var path in Directory.EnumerateFiles(directoryPath, $"{fileKey}*.*"))
                {
                    File.Delete(path);
                }
            }
        }
    }

    internal interface IFileCache
    {
        string GetCachedFileContent(int repositoryId, string repositoryName, string filePath, string sha, Func<int, string, string> loadContent);
    }
}
