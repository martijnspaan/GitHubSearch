using System;
using System.IO;
using FakeItEasy;
using Xunit;

using FluentAssertions;
using Octokit;

namespace GitHubSearch.Specs
{    
    public class FileCacheSpecs
    {
        [Fact]
        public void When_instantiated_it_should_create_cache_folder()
        {
            // Arrange
            if (Directory.Exists(".\\Cache"))
            {
                Directory.Delete(".\\Cache");
            }

            // Act
            var fileCache = new FileCache();

            // Assert
            Directory.Exists(".\\Cache").Should().BeTrue();
        }

        [Fact]
        public void When_cached_file_exists_it_should_return_cached_content()
        {
            // Arrange
            var fileCache = new FileCache();

            var searchCode = new SearchCodeBuilder().Build();

            var downloadContentFunc = A.Fake<Func<SearchCode, string>>();

            // Act
            fileCache.GetCachedFileContent(searchCode, downloadContentFunc);

            // Assert
            A.CallTo(() => downloadContentFunc.Invoke(A<SearchCode>.Ignored)).MustNotHaveHappened();
        }

        public class SearchCodeBuilder
        {
            public SearchCode Build()
            {
                return null;
            }
        }
        
    }
}
