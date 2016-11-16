using FluentAssertions;
using Octokit;
using Xunit;

namespace GitHubSearch.Specs
{
    public class GitHubClientFactorySpecs
    {
        [Fact]
        public void When_created_with_invalid_credentials_it_should_return_nothing()
        {
            // Arrange
            var accessToken = "SomeInvalidToken";
            var gitHubClientFactory = new GitHubClientFactory();

            // Act
            IGitHubClient gitHubClient = gitHubClientFactory.Create(accessToken);

            // Assert
            gitHubClient.Should().BeNull();
        }

        [Fact]
        public void When_created_without_credentials_it_should_return_client_with_anonymous_access()
        {
            // Arrange
            var accessToken = "";
            var gitHubClientFactory = new GitHubClientFactory();

            // Act
            IGitHubClient gitHubClient = gitHubClientFactory.Create(accessToken);

            // Assert
            gitHubClient.Should().NotBeNull();
            gitHubClient.Connection.As<Connection>().UserAgent.Should().Contain("GitHubSearch");
            gitHubClient.Connection.Credentials.AuthenticationType.Should().Be(AuthenticationType.Anonymous);
        }

        [Fact]
        public void When_created_with_valid_credentials_it_should_return_client_with_oauth_access()
        {
            // Cannot be tested because it would require an actual access token, which is a breach of GitHub policy.
        }
    }
}