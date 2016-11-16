using System;
using FakeItEasy;
using Xunit;

using FluentAssertions;

namespace GitHubSearch.Specs
{
    public class FileCacheSpecs
    {
        private static readonly string RelativeCacheFolder = "\\GitHubSearch\\Cache";

        private static readonly string AbsoluteCacheFolder =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}{RelativeCacheFolder}";

        [Fact]
        public void When_instantiated_it_should_create_cache_folder()
        {
            // Arrange
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.DirectoryExists(AbsoluteCacheFolder)).Returns(false);

            // Act
            var fileCache = new FileCache(fileSystem);

            // Assert
            A.CallTo(() => fileSystem.EnsureDirectoryExists(AbsoluteCacheFolder)).MustHaveHappened();
        }

        [Fact]
        public void When_cached_file_does_not_exists_it_should_be_created()
        {
            // Arrange
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.FileExists(A<string>._)).Returns(false);

            var fileCache = new FileCache(fileSystem);

            var filePath = "SomeFile.txt";
            int repositoryId = 123;
            string repositoryName = "SomeRepo";

            var sha = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";

            var content = "SomeContent";

            var expectedFilePathPostfix = $"{RelativeCacheFolder}\\{repositoryName}\\{filePath}-{sha}.cache";

            var downloadContentFunc = A.Fake<Func<int, string, string>>();
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).Returns(content);

            // Act
            fileCache.GetCachedFileContent(repositoryId, repositoryName, filePath, sha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(123, filePath)).MustHaveHappened();
            A.CallTo(() => fileSystem.WriteAllText(A<string>.That.EndsWith(expectedFilePathPostfix), content)).MustHaveHappened();
        }

        [Fact]
        public void When_cached_file_exists_it_should_be_loaded_from_cache()
        {
            // Arrange
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.FileExists(A<string>._)).Returns(true);

            var fileCache = new FileCache(fileSystem);

            int repositoryId = 123;
            string repositoryName = "SomeRepo";
            var filePath = "SomeFile.txt";
            var sha = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";

            var expectedFilePathPostfix = $"{RelativeCacheFolder}\\{repositoryName}\\{filePath}-{sha}.cache";

            string content = "SomeContent";
            A.CallTo(() => fileSystem.ReadAllText(A<string>.That.EndsWith(expectedFilePathPostfix))).Returns(content);

            var downloadContentFunc = A.Fake<Func<int, string, string>>();

            // Act
            var cachedContent = fileCache.GetCachedFileContent(repositoryId, repositoryName, filePath, sha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustNotHaveHappened();

            cachedContent.Should().Be(content);
        }

        [Fact]
        public void When_file_is_downloaded_it_should_clear_old_cache_files()
        {
            // Arrange
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.FileExists(A<string>._)).Returns(false);

            var fileCache = new FileCache(fileSystem);

            int repositoryId = 123;
            string repositoryName = "SomeRepo";

            string filePath = "SomeFile.txt";

            string oldSha1 = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";
            string oldSha2 = "6ce7bf64673d40e7b5b3d13f4f1f992c1ea37119";
            string newSha = "b12351820c868918f4df7cfef35b7bc70a878bd5";

            string oldContent = "SomeOldContent";
            string newContent = "SomeNewContent";

            string oldCacheFile1 = $"{RelativeCacheFolder}\\{repositoryName}\\{filePath}-{oldSha1}.cache";
            string oldCacheFile2 = $"{RelativeCacheFolder}\\{repositoryName}\\{filePath}-{oldSha2}.cache";
            string newCacheFile = $"{RelativeCacheFolder}\\{repositoryName}\\{filePath}-{newSha}.cache";

            string repositoryCacheDirectory = $"{RelativeCacheFolder}\\{repositoryName}";
            A.CallTo(() => fileSystem.EnumerateFiles(A<string>.That.EndsWith(repositoryCacheDirectory), A<string>._))
                .Returns(new [] { oldCacheFile1, oldCacheFile2 });
            A.CallTo(() => fileSystem.ReadAllText(A<string>.That.EndsWith(oldCacheFile1))).Returns(oldContent);
            A.CallTo(() => fileSystem.ReadAllText(A<string>.That.EndsWith(oldCacheFile2))).Returns(oldContent);

            var downloadContentFunc = A.Fake<Func<int, string, string>>();
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).Returns(newContent);

            // Act
            fileCache.GetCachedFileContent(repositoryId, repositoryName, filePath, newSha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustHaveHappened();
            A.CallTo(() => fileSystem.DeleteFile(A<string>.That.EndsWith(oldCacheFile1))).MustHaveHappened();
            A.CallTo(() => fileSystem.DeleteFile(A<string>.That.EndsWith(oldCacheFile2))).MustHaveHappened();
            A.CallTo(() => fileSystem.WriteAllText(A<string>.That.EndsWith(newCacheFile), newContent)).MustHaveHappened();
        }

        [Fact]
        public void When_flushing_cache_it_should_clear_all_cache_files()
        {
            // Arrange
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.FileExists(A<string>._)).Returns(false);

            var fileCache = new FileCache(fileSystem);
            
            // Act
            fileCache.Flush();

            // Assert
            A.CallTo(() => fileSystem.RemoveFolder(A<string>.That.EndsWith(RelativeCacheFolder))).MustHaveHappened();
        }
    }
}