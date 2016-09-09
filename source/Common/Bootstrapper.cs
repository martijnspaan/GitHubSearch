using System;
using TinyIoC;

namespace GitHubSearch.Common
{
    internal class Bootstrapper
    {
        public static TinyIoCContainer Start()
        {
            var container = TinyIoCContainer.Current;

            container.Register<IFileCache, FileCache>();
            container.Register<IConfiguration, Configuration>();
            container.Register<IGitHubClientFactory, GitHubClientFactory>();
            container.Register<IGitHubAdapter, GitHubAdapter>();
            container.Register<IConfigurationSearcher, ConfigurationSearcher>();

            var configuration = container.Resolve<IConfiguration>();
            var adapter = container.Resolve<IGitHubAdapter>();

            InitGithubAccessToken(adapter, configuration);

            return container;
        }

        private static void InitGithubAccessToken(IGitHubAdapter adapter, IConfiguration configuration)
        {
            var accessToken = configuration.GithubAccessToken;

            while (string.IsNullOrEmpty(accessToken) || !adapter.InitAccessToken(accessToken))
            {
                accessToken = AskForAccessToken();

                configuration.StoreAccessToken(accessToken);
            }
        }

        private static string AskForAccessToken()
        {
            Console.WriteLine();
            Console.WriteLine(" Cannot find a valid GitHub access token.");
            Console.WriteLine();
            Console.WriteLine(" Please provide an access token to your own account in order to iterate the private repositories.");
            Console.WriteLine(" This token will have minimal credentials and only used to access GitHub to perform the search query.");
            Console.WriteLine(" Create a token by going to https://github.com/settings/tokens and create a token with \"Full control of private repositories\"");
            Console.WriteLine();
            Console.Write(" GitHub accesstoken: ");
            return Console.ReadLine();
        }
    }
}