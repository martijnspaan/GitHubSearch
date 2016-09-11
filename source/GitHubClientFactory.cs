using Octokit;
using Octokit.Internal;

namespace GitHubSearch
{
    internal class GitHubClientFactory : IGitHubClientFactory
    {
        private static readonly ProductHeaderValue ProductHeader = new ProductHeaderValue("GitHubSearch", Program.ProductVersion);

        public IGitHubClient Create(string accessToken)
        {
            IGitHubClient gitHubClient;
            if (!string.IsNullOrEmpty(accessToken))
            {
                var credentialsStore = new InMemoryCredentialStore(new Credentials(accessToken));
                gitHubClient = new GitHubClient(ProductHeader, credentialsStore);
            }
            else
            {
                gitHubClient = new GitHubClient(ProductHeader);
            }

            return ReturnClientWhenValid(gitHubClient);
        }

        private static IGitHubClient ReturnClientWhenValid(IGitHubClient gitHubClient)
        {
            try
            {
                gitHubClient.Repository.Get("martijnspaan", "GitHubSearch").Wait();
            }
            catch
            {
                return null;
            }

            return gitHubClient;
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
