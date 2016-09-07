using System;
using System.Collections.Generic;

using Octokit;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;

namespace GitHubSearch
{
    /// <summary>
    /// Adapter for the Octokit library, exposing specific features needed searching.
    /// </summary>
    internal class GitHubAdapter : IGitHubAdapter
    {
        public int CurrentSearchItemsCount { get; private set; }

        public string GitHubTargetName => gitHubTargetName;

        private static readonly string gitHubTargetName = ConfigurationManager.AppSettings["GithubTargetName"];

        private static readonly Uri accountUri = new Uri("https://github.com/" + gitHubTargetName);

        private readonly GitHubClient _client = new GitHubClient(new ProductHeaderValue("GitHubSearch"), accountUri);

        private readonly IFileCache _cache = new FileCache();

        public bool InitAccessToken(string accessToken)
        {
            _client.Credentials = new Credentials(accessToken);

            User user;
            try
            {
                user = _client.User.Current().Result;
            }
            catch
            {
                return false;
            }

            return user != null;
        }

        public IEnumerable<Repository> FindRepositories()
        {
            IReadOnlyList<Repository> repositories;
            try
            {
                var user = _client.User.Get(gitHubTargetName).Result;

                repositories = user.Type == AccountType.Organization
                    ? _client.Repository.GetAllForOrg(user.Login).Result
                    : _client.Repository.GetAllForUser(user.Login).Result;
            }
            catch
            {
                throw new Exception($" Specified target '{GitHubTargetName}' is neither a valid organization or user.");
            }

            string[] filters = GetRepositoryFilters();

            return
                repositories.Where(
                    x => filters.Any(filter => Regex.IsMatch(x.Name, filter, RegexOptions.IgnoreCase)));
        }

        private string[] GetRepositoryFilters()
        {
            var repositoryFilters = ConfigurationManager.AppSettings["RepositoryFilters"];
            return repositoryFilters.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"^{x}$").ToArray();
        }
        
        public IEnumerable<ConfigFileHit> DownloadConfigurationFiles(IEnumerable<Repository> repos, string searchToken)
        {
            SearchCodeResult result = FindConfigurationFiles(repos, searchToken);
            
            // Because of yield, cannot return the total count
            CurrentSearchItemsCount = result.TotalCount;

            foreach (SearchCode configFileInfo in result.Items)
            {
                yield return new ConfigFileHit
                {
                    RepositoryName = configFileInfo.Repository.Name,
                    Path = configFileInfo.Path,
                    HtmlUrl = configFileInfo.HtmlUrl,
                    Content = _cache.GetCachedFileContent(configFileInfo, DownloadFileContent)
                };           
            }
        }

        private SearchCodeResult FindConfigurationFiles(IEnumerable<Repository> repos, string searchToken)
        {
            var filename = ConfigurationManager.AppSettings["FilenameFilter"];
            var request = new SearchCodeRequest(searchToken);
            foreach (var repo in repos)
            {
                request.Repos.Add(repo.FullName);
            }
            request.FileName = filename;

            
            var result =  _client.Search.SearchCode(request).Result;
            return result;
        }

        private string DownloadFileContent(SearchCode configFileInfo)
        {
            return _client.Repository.Content.GetAllContents(configFileInfo.Repository.Id, configFileInfo.Path).Result.First().Content;
        }
    }

    /// <summary>
    /// Adapter for the GitHub Api.
    /// </summary>
    /// <remarks>Used Octokit for consulting the GitHub Api.</remarks>
    public interface IGitHubAdapter
    {
        /// <summary>
        /// The name of the account that is being searched.
        /// </summary>
        string GitHubTargetName { get; }

        /// <summary>
        /// The current amount of items found by the search.
        /// </summary>
        int CurrentSearchItemsCount { get; }

        /// <summary>
        /// Initialized the access token used to authenticate with the GitHub API.
        /// </summary>        
        /// <returns>Returns true when the access token is valid, otherwise false.</returns>
        bool InitAccessToken(string accessToken);

        /// <summary>
        /// Find a list of repositories that comply to the repositories filter.
        /// </summary>
        IEnumerable<Repository> FindRepositories();

        /// <summary>
        /// Lazily yields a list of downloaded configuration files.
        /// </summary>
        IEnumerable<ConfigFileHit> DownloadConfigurationFiles(IEnumerable<Repository> repos, string searchToken);
    }
}