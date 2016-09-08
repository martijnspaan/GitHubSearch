using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Drawing;

namespace GitHubSearch
{
    internal class ConfigurationSearcher : IConfigurationSearcher
    {
        private IGitHubAdapter _gitHubAdapter;

        public ConfigurationSearcher(IGitHubAdapter gitHubAdapter)
        {
            _gitHubAdapter = gitHubAdapter;
        }

        public void SearchFor(string searchToken)
        {
            IEnumerable<ConfigFileHit> configurationFiles = FindAllConfigurationFiles(searchToken);            

            configurationFiles = FilterConfigurationFilesWithSearchToken(configurationFiles, searchToken);            

            LogHits(configurationFiles, searchToken);
        }

        private IEnumerable<ConfigFileHit> FindAllConfigurationFiles(string searchToken)
        {
            Console.WriteLine();
            Console.WriteLine($" Searching GitHub for repositories on {_gitHubAdapter.GitHubTargetName}");

            string[] repos = _gitHubAdapter.FindRepositories();

            Console.WriteLine();
            Console.WriteLine($" Searching through {repos.Length} repositories for '{searchToken}'");

            return _gitHubAdapter.DownloadConfigurationFiles(repos, searchToken);
        }

        private IEnumerable<ConfigFileHit> FilterConfigurationFilesWithSearchToken(IEnumerable<ConfigFileHit> configurationFiles, string searchToken)
        {
            ConcurrentBag<ConfigFileHit> hits = new ConcurrentBag<ConfigFileHit>();

            ProgressIndicator.Start();

            Parallel.ForEach(configurationFiles, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (configFile) =>
            {
                ProgressIndicator.SetTotal(_gitHubAdapter.CurrentSearchItemsCount);

                if (FindAllOccurences(configFile, searchToken))
                {
                    hits.Add(configFile);
                }

                ProgressIndicator.Step();
            });

            ProgressIndicator.WaitTillFinished();

            return hits;
        }

        private bool FindAllOccurences(ConfigFileHit configFile, string searchToken)
        {
            int lineNr = 1;

            configFile.FoundLineNumbers = new List<int>();
            foreach (var line in configFile.Content.Split('\n'))
            {
                if (Regex.IsMatch(line, searchToken, RegexOptions.IgnoreCase))
                {
                    configFile.FoundLineNumbers.Add(lineNr);
                }

                lineNr++;
            }
            return configFile.FoundLineNumbers.Any();
        }

        private void LogHits(IEnumerable<ConfigFileHit> configurationFiles, string searchToken)
        {
            if (!configurationFiles.Any())
            {
                Console.WriteLine($" Search token has not been found in any repository.");
                return;
            }

            string lastRepo = string.Empty;
            foreach (var hit in configurationFiles.OrderBy(x => x.RepositoryName))
            {
                if (lastRepo != hit.RepositoryName)
                {
                    lastRepo = hit.RepositoryName;

                    Console.WriteLine();
                    Colorful.Console.WriteLineFormatted(" Found on {0} in files:", Color.IndianRed, Color.DarkGray, hit.RepositoryName);
                }

                LogHit(hit, searchToken);
            }
        }

        private void LogHit(ConfigFileHit hit, string searchToken)
        {
            LogHitFilename(hit);

            LogHitLines(hit, searchToken);
        }

        private static void LogHitFilename(ConfigFileHit hit)
        {
            switch (ConfigurationManager.AppSettings["OutputMode"])
            {
                case "HtmlUrl":
                    Console.WriteLine($" {hit.FoundLineNumbers.Count()} hits in {hit.HtmlUrl}");
                    break;
                default:
                    Console.WriteLine($" {hit.FoundLineNumbers.Count()} hits in {hit.Path}");
                    break;
            }
        }

        private void LogHitLines(ConfigFileHit hit, string searchToken)
        {
            var showLinesCount = int.Parse(ConfigurationManager.AppSettings["SurroundingLines"]);

            var lines = hit.Content.Split('\n');
            foreach (var line in hit.FoundLineNumbers)
            {
                var start = Math.Max(line - showLinesCount - 1, 0);
                var end = Math.Min(line + showLinesCount, lines.Length);

                for (int i = start; i < end; i++)
                {
                    OutputWithHighlightedSearchtoken($"{i,5}: {lines[i]}", searchToken);
                }

                Console.WriteLine(new string('-', 70));
            }
        }

        private void OutputWithHighlightedSearchtoken(string text, string searchToken)
        {
            Colorful.StyleSheet styleSheet = new Colorful.StyleSheet(Color.White);
            styleSheet.AddStyle(GenerateCaseInsensitiveSearchTokenMatcher(searchToken), Color.MediumSlateBlue);

            Colorful.Console.WriteLineStyled(text, styleSheet);
        }        

        private string GenerateCaseInsensitiveSearchTokenMatcher(string searchToken)
        {
            return searchToken.Aggregate(string.Empty, 
                (value, c) => value += $"[{c.ToString().ToLower()}{c.ToString().ToUpper()}]");
        }
    }

    /// <summary>
    /// Performs the search on GitHub.
    /// </summary>
    internal interface IConfigurationSearcher
    {
        /// <summary>
        /// Searches for the specified search token.
        /// </summary>        
        void SearchFor(string searchToken);
    }
}
