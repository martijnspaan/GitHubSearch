using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Octokit;
using Xunit;

namespace GitHubSearch.Specs
{
    public class GitHubClientFactorySpecs
    {
        [Fact]
        public void When_created_it_should_return_github_client_interface()
        {
            // Arrange
            var accessToken = "6ced294675836d90e55e5c0ebe5cd7b1debb109b";
            var gitHubClientFactory = new GitHubClientFactory();

            // Act
            IGitHubClient gitHubClient = gitHubClientFactory.Create(accessToken);

            // Assert
            gitHubClient.Should().NotBeNull();
            gitHubClient.Connection.As<Connection>().UserAgent.Should().Contain("GitHubSearch");
            gitHubClient.Connection.Credentials.AuthenticationType.Should().Be(AuthenticationType.Oauth);
            gitHubClient.Connection.Credentials.Password.Should().Be(accessToken);
        }
    }
}