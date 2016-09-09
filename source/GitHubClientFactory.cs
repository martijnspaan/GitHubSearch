using System;
using System.Reflection;
using Octokit;
using Octokit.Internal;

namespace GitHubSearch
{
    internal class GitHubClientFactory : IGitHubClientFactory
    {
        private static readonly ProductHeaderValue ProductHeader = new ProductHeaderValue("GitHubSearch", Program.ProductVersion);

        public IGitHubClient Create(string accessToken)
        {
            var credentialsStore = new InMemoryCredentialStore(new Credentials(accessToken));

            return new GitHubClient(ProductHeader, credentialsStore);
        }
    }

    /// <summary>
    /// Creates a valid authorized GitHub client.
    /// </summary>
    internal interface IGitHubClientFactory
    {
        IGitHubClient Create(string accessToken);
    }
}
