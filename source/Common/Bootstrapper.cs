using System;
using System.Collections.Generic;
using System.Linq;
using TinyIoC;

namespace GitHubSearch.Common
{
    internal class Bootstrapper
    {
        private static readonly IDictionary<string, Func<IConfiguration, bool>> RequiredConfigurationChecks =
            new Dictionary <string, Func<IConfiguration, bool>>
            {
                {"GithubTargetName", c => !string.IsNullOrEmpty(c.GithubTargetName)},
                {"RepositoryFilters", c => c.RepositoryFilters.Any()},
                {"FilenameFilter", c => !string.IsNullOrEmpty(c.FilenameFilter)}
            };

        public static TinyIoCContainer Start(Options options)
        {
            var container = TinyIoCContainer.Current;

            container.Register(options);

            container.Register<IFileSystem, FileSystem>();
            container.Register<IFileCache, FileCache>();
            container.Register<IConfiguration, Configuration>();
            container.Register<IGitHubClientFactory, GitHubClientFactory>();
            container.Register<IGitHubAdapter, GitHubAdapter>();
            container.Register<IFileSearcher, FileSearcher>();

            var configuration = container.Resolve<IConfiguration>();
            var adapter = container.Resolve<IGitHubAdapter>();

            InitGithubAccessToken(adapter, configuration);

            AssertRequiredSettingsHaveValues(configuration);

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

        private static void AssertRequiredSettingsHaveValues(IConfiguration configuration)
        {
            var missingSettings = new List<string>();
            foreach (var requiredSettingCheck in RequiredConfigurationChecks)
            {
                if (!requiredSettingCheck.Value(configuration))
                {
                    missingSettings.Add(requiredSettingCheck.Key);
                }
            }

            if (missingSettings.Any())
            {
                Console.WriteLine();
                Console.WriteLine(" The following required settings do not have a valid value:");
                Console.WriteLine(" " + string.Join(", ", missingSettings));

                throw new Exception(" Provide a value in GithubSearch.exe.config");
            }
        }
    }
}