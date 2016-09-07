using System;
using Octokit;
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
        public string GetCachedFileContent(SearchCode searchHit, Func<SearchCode, string> loadContent)
        {
            var cacheKey = CreateCacheKey(searchHit);

            var cachedFilePath = $"{LocalCacheFolder}\\{cacheKey}.cache";

            if (File.Exists(cachedFilePath))
            {
                return File.ReadAllText(cachedFilePath);
            }

            var actualContent = loadContent(searchHit);

            ClearCacheFor(searchHit);

            StoreInCache(cachedFilePath, actualContent);

            return actualContent;
        }

        private static void StoreInCache(string cachedFilePath, string actualContent)
        {
            File.WriteAllText(cachedFilePath, actualContent);
        }

        private string CreateCacheKey(SearchCode searchHit)
        {
            var fileKey = searchHit.Path.Replace("/", "-");
            return $"{searchHit.Repository.Id}-{fileKey}-{searchHit.Sha}";
        }

        private static void ClearCacheFor(SearchCode searchHit)
        {
            var fileKey = searchHit.Path.Replace("/", "-");
            foreach (var filePath in Directory.EnumerateFiles(LocalCacheFolder, $"{searchHit.Repository.Id}-{fileKey}*.*"))
            {
                File.Delete(filePath);
            }
        }
    }

    internal interface IFileCache
    {
        string GetCachedFileContent(SearchCode searchHit, Func<SearchCode, string> loadContent);
    }
}
