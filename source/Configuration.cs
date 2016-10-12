using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GitHubSearch
{
    internal class Configuration : IConfiguration
    {
        private readonly Options options;

        private string _outputMode;
        private int? _surroundingLines;
        private string _githubTargetName;
        private string[] _repositoryFilters;
        private string _filenameFilter;
        private string _githubAccessToken;

        public string OutputMode => _outputMode ?? (_outputMode = options.OutputMode ?? ConfigurationManager.AppSettings["OutputMode"]);

        public int SurroundingLines => (_surroundingLines ?? (_surroundingLines = options.SurroundingLines ?? int.Parse(ConfigurationManager.AppSettings["SurroundingLines"]))).Value;

        public string GithubTargetName => _githubTargetName ?? (_githubTargetName = options.GithubTargetName ?? ConfigurationManager.AppSettings["GithubTargetName"]);

        public string[] RepositoryFilters => _repositoryFilters ?? (_repositoryFilters = GetRepositoryFilters());

        public string FilenameFilter => _filenameFilter ?? (_filenameFilter = options.FilenameFilter ?? ConfigurationManager.AppSettings["FilenameFilter"]);

        public string GithubAccessToken => _githubAccessToken ?? (_githubAccessToken = ConfigurationManager.AppSettings["GithubAccessToken"]);

        public Configuration(Options options)
        {
            this.options = options;
        }

        private string[] GetRepositoryFilters()
        {
            var repositoryFilters = options.RepositoryFilters ?? ConfigurationManager.AppSettings["RepositoryFilters"];
            return repositoryFilters.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => $"^{x}$")
                .ToArray();
        }

        public void StoreAccessToken(string accessToken)
        {
            try
            {
                var configDoc = XDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, LoadOptions.PreserveWhitespace);

                var accessTokenNode = configDoc.XPathSelectElement("//appSettings/add[@key='GithubAccessToken']");
                XAttribute xAttribute = accessTokenNode.Attribute("value");
                if (xAttribute != null)
                {
                    xAttribute.Value = accessToken;
                }
                configDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine(" Failed to store access token in configuration file " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, Color.Red);
                Colorful.Console.WriteLine(" " + ex.Message, Color.Red);
            }
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