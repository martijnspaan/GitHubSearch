using FakeItEasy;
using FluentAssertions;
using Octokit;
using Xunit;

namespace GitHubSearch.Specs
{
    public class GitHubAdapterSpecs
    {
        [Fact]
        public void When_initialized_sucessfully_it_should_return_true()
        {
            // Arrange
            var accessToken = "SomeValidAccessToken";

            var clientFactory = A.Fake<IGitHubClientFactory>();
            var configuration = A.Fake<IConfiguration>();
            var fileCache = A.Fake<IFileCache>();
            var gitHubClient = A.Fake<IGitHubClient>();

            A.CallTo(() => clientFactory.Create(accessToken)).Returns(gitHubClient);

            var gitHubAdapter = new GitHubAdapter(clientFactory, configuration, fileCache, new Options());

            // Act
            bool result = gitHubAdapter.InitAccessToken(accessToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void When_not_initialized_sucessfully_it_should_return_false()
        {
            // Arrange
            var accessToken = "SomeInvalidAccessToken";

            var clientFactory = A.Fake<IGitHubClientFactory>();
            var configuration = A.Fake<IConfiguration>();
            var fileCache = A.Fake<IFileCache>();

            A.CallTo(() => clientFactory.Create(accessToken)).Returns(null);

            var gitHubAdapter = new GitHubAdapter(clientFactory, configuration, fileCache, new Options());

            // Act
            bool result = gitHubAdapter.InitAccessToken(accessToken);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void When_finding_repositories_for_user_it_should_return_them()
        {
            // Todo: Avoid integration tests with Octokit by introducing actual adapter class

            /*
            // Arrange
            var accessToken = "SomeValidAccessToken";

            var clientFactory = A.Fake<IGitHubClientFactory>();
            var configuration = A.Fake<IConfiguration>();
            var fileCache = A.Fake<IFileCache>();
            var gitHubClient = A.Fake<IGitHubClient>();
            var usersClient = A.Fake<IUsersClient>();

            A.CallTo(() => gitHubClient.User).Returns(usersClient);
            A.CallTo(() => usersClient.Get(A<string>._)).Returns(Task.FromResult(new User()));

            A.CallTo(() => clientFactory.Create(accessToken)).Returns(gitHubClient);

            var gitHubAdapter = new GitHubAdapter(clientFactory, configuration, fileCache);

            gitHubAdapter.InitAccessToken(accessToken);

            // Act
            var foundRepositories = gitHubAdapter.FindRepositories();

            // Assert
            foundRepositories.Should().HaveCount(2);
            */
        }
    }
}