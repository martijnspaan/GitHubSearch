using System;
using System.Collections.Generic;

using Octokit;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitHubSearch
{
    /// <summary>
    /// Adapter for the Octokit library, exposing specific features needed searching.
    /// </summary>
    internal class GitHubAdapter : IGitHubAdapter
    {
        private readonly IGitHubClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IFileCache _cache;

        public int CurrentSearchItemsCount { get; private set; }

        private IGitHubClient _client;


        public GitHubAdapter(IGitHubClientFactory clientFactory, IConfiguration configuration, IFileCache cache)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _cache = cache;
        }

        public bool InitAccessToken(string accessToken)
        {
            _client = _clientFactory.Create(accessToken);

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

        public string[] FindRepositories()
        {
            IReadOnlyList<Repository> repositories;
            try
            {
                var user = _client.User.Get(_configuration.GithubTargetName).Result;

                repositories = user.Type == AccountType.Organization
                    ? _client.Repository.GetAllForOrg(user.Login).Result
                    : _client.Repository.GetAllForUser(user.Login).Result;
            }
            catch
            {
                throw new Exception($" Specified target '{_configuration.GithubTargetName}' is neither a valid organization or user.");
            }

            return repositories
                .Where(x => _configuration.RepositoryFilters.Any(filter => Regex.IsMatch(x.Name, filter, RegexOptions.IgnoreCase)))
                .Select(x => x.FullName)
                .ToArray();
        }

        public IEnumerable<ConfigFileHit> DownloadConfigurationFiles(string[] repos, string searchToken)
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
                    Content = _cache.GetCachedFileContent(configFileInfo.Repository.Id, configFileInfo.Path, configFileInfo.Sha, DownloadFileContent)
                };
            }
        }

        private SearchCodeResult FindConfigurationFiles(string[] repos, string searchToken)
        {
            var request = new SearchCodeRequest(searchToken);
            foreach (var repo in repos)
            {
                request.Repos.Add(repo);
            }
            request.FileName = _configuration.FilenameFilter;

            try
            {
                return _client.Search.SearchCode(request).Result;
            }
            catch (Exception)
            {
                throw new Exception("Search resulted in a server error. Probably caused by a too large result set. Please refine your search and try again.");
            }

        }

        private string DownloadFileContent(int repositoryId, string filePath)
        {
            return _client.Repository.Content.GetAllContents(repositoryId, filePath).Result.First().Content;
        }
    }

    /// <summary>
    /// Adapter for the GitHub Api.
    /// </summary>
    /// <remarks>Used Octokit for consulting the GitHub Api.</remarks>
    public interface IGitHubAdapter
    {
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
        string[] FindRepositories();

        /// <summary>
        /// Lazily yields a list of downloaded configuration files.
        /// </summary>
        IEnumerable<ConfigFileHit> DownloadConfigurationFiles(string[] repos, string searchToken);
    }
}