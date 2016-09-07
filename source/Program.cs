using System;
using System.Configuration;
using System.Reflection;

namespace GitHubSearch
{
    internal class Program
    {
        private static readonly IGitHubAdapter _gitHubAdapter = new GitHubAdapter();

        private static readonly ConfigurationSearcher _configurationSearcher = new ConfigurationSearcher(_gitHubAdapter);

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine($" GitHubSearch version {ProductVersion}");

            if (args.Length <= 0)
            {
                Console.WriteLine();
                Console.WriteLine(@" Usage: GitHubSearch.exe ""<searchtoken>""");
                Environment.Exit(1);
            }

            InitGithubAccessToken();

            try
            {
                _configurationSearcher.SearchFor(args[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
            }
        }

        internal static string ProductVersion
        {
            get
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Revision}";
            }
        }

        internal static void InitGithubAccessToken()
        {
            var accessToken = ConfigurationManager.AppSettings["GithubAccessToken"];

            while (string.IsNullOrEmpty(accessToken) || !_gitHubAdapter.InitAccessToken(accessToken))
            {
                accessToken = AskForAccessToken();

                StoreAccessToken(accessToken);
            }
        }

        internal static string AskForAccessToken()
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

        internal static void StoreAccessToken(string accessToken)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["GithubAccessToken"].Value = accessToken;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}