using System;
using System.Configuration;
using System.Linq;

namespace GitHubSearch
{
    internal class Configuration : IConfiguration
    {
        private string _outputMode;
        private int? _surroundingLines;
        private string _githubTargetName;
        private string[] _repositoryFilters;
        private string _filenameFilter;
        private string _githubAccessToken;

        public string OutputMode => _outputMode ?? (_outputMode = ConfigurationManager.AppSettings["OutputMode"]);

        public int SurroundingLines => (_surroundingLines ?? (_surroundingLines = int.Parse(ConfigurationManager.AppSettings["SurroundingLines"]))).Value;

        public string GithubTargetName => _githubTargetName ?? (_githubTargetName = ConfigurationManager.AppSettings["GithubTargetName"]);

        public string[] RepositoryFilters => _repositoryFilters ?? (_repositoryFilters = GetRepositoryFilters());

        public string FilenameFilter => _filenameFilter ?? (_filenameFilter = ConfigurationManager.AppSettings["FilenameFilter"]);

        public string GithubAccessToken => _githubAccessToken ?? (_githubAccessToken = ConfigurationManager.AppSettings["GithubAccessToken"]);
        
        private string[] GetRepositoryFilters()
        {
            var repositoryFilters = ConfigurationManager.AppSettings["RepositoryFilters"];
            return repositoryFilters.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => $"^{x}$")
                .ToArray();
        }



        public void StoreAccessToken(string accessToken)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["GithubAccessToken"].Value = accessToken;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    internal interface IConfiguration
    {
        string OutputMode { get; }
        int SurroundingLines { get; }
        string GithubTargetName { get; }
        string[] RepositoryFilters { get; }
        string FilenameFilter { get; }
        string GithubAccessToken { get; }

        void StoreAccessToken(string accessToken);
    }
}
