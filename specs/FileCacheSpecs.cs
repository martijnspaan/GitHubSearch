using System;
using System.IO;
using FakeItEasy;
using Xunit;

using FluentAssertions;

namespace GitHubSearch.Specs
{    
    public class FileCacheSpecs
    {
        public FileCacheSpecs()
        {
            if (Directory.Exists(".\\Cache"))
            {
                Directory.Delete(".\\Cache", true);
            }
        }

        [Fact]
        public void When_instantiated_it_should_create_cache_folder()
        {
            // Act
            var fileCache = new FileCache();

            // Assert
            Directory.Exists(".\\Cache").Should().BeTrue();
        }

        [Fact]
        public void When_cached_file_does_not_exists_it_should_return_cached_content()
        {
            // Arrange
            var fileCache = new FileCache();

            var repositoryId = 123;

            var filePath = "SomeFile.txt";

            var sha = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";

            var downloadContentFunc = A.Fake<Func<int, string, string>>();

            // Act
            fileCache.GetCachedFileContent(repositoryId, filePath, sha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustHaveHappened();
        }

        [Fact]
        public void When_cached_file_does_not_exists_it_should_be_created()
        {
            // Arrange
            var fileCache = new FileCache();

            var repositoryId = 123;

            var filePath = "SomeFile.txt";

            var sha = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";

            var content = "SomeContent";

            var expectedFilePath = $".\\Cache\\{repositoryId}-{filePath}-{sha}.cache";

            var downloadContentFunc = A.Fake<Func<int, string, string>>();
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).Returns(content);

            // Act
            fileCache.GetCachedFileContent(repositoryId, filePath, sha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustHaveHappened();
            File.Exists(expectedFilePath).Should().BeTrue();
            File.ReadAllText(expectedFilePath).Should().Be(content);
        }

        [Fact]
        public void When_cached_file_exists_it_should_be_loaded_from_cache()
        {
            // Arrange
            var fileCache = new FileCache();

            var repositoryId = 123;

            var filePath = "SomeFile.txt";

            var sha = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";

            var content = "SomeContent";

            var expectedFilePath = $".\\Cache\\{repositoryId}-{filePath}-{sha}.cache";

            var downloadContentFunc = A.Fake<Func<int, string, string>>();

            // Act
            File.WriteAllText(expectedFilePath, content);
            var cachedContent = fileCache.GetCachedFileContent(repositoryId, filePath, sha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustNotHaveHappened();

            cachedContent.Should().Be(content);
        }

        [Fact]
        public void When_file_is_downloaded_it_should_clear_old_cache_files()
        {
            // Arrange
            var fileCache = new FileCache();

            var repositoryId = 123;

            var filePath = "SomeFile.txt";

            var oldSha1 = "8560328effd5fbee640e8aaef79b5ebc7ba1afe5";
            var oldSha2 = "6ce7bf64673d40e7b5b3d13f4f1f992c1ea37119";
            var newSha = "b12351820c868918f4df7cfef35b7bc70a878bd5";

            var oldContent = "SomeOldContent";
            var newContent = "SomeNewContent";

            var oldCacheFile1 = $".\\Cache\\{repositoryId}-{filePath}-{oldSha1}.cache";
            var oldCacheFile2 = $".\\Cache\\{repositoryId}-{filePath}-{oldSha2}.cache";
            var newCacheFile = $".\\Cache\\{repositoryId}-{filePath}-{newSha}.cache";

            var downloadContentFunc = A.Fake<Func<int, string, string>>();
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).Returns(newContent);

            // Act
            File.WriteAllText(oldCacheFile1, oldContent);
            File.WriteAllText(oldCacheFile2, oldContent);
            fileCache.GetCachedFileContent(repositoryId, filePath, newSha, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<int>._, A<string>._)).MustHaveHappened();
            File.Exists(oldCacheFile1).Should().BeFalse();
            File.Exists(oldCacheFile2).Should().BeFalse();
            File.Exists(newCacheFile).Should().BeTrue();
            File.ReadAllText(newCacheFile).Should().Be(newContent);
        }
    }
}
