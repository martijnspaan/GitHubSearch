using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace GitHubSearch
{
    internal class FileSearcher : IFileSearcher
    {
        private readonly IGitHubAdapter _gitHubAdapter;

        private readonly IConfiguration _configuration;

        public FileSearcher(IGitHubAdapter gitHubAdapter, IConfiguration configuration)
        {
            _gitHubAdapter = gitHubAdapter;
            _configuration = configuration;
        }

        public void Search(Options options)
        {
            IEnumerable<FileHit> matchingFiles = FindAllMatchingFiles();

            Console.WriteLine($" Start searching for '{options.SearchToken}'");
            FileHit[] hitFiles = FilterFilesWithSearchToken(matchingFiles, options.SearchToken);

            if (!options.SuppressLoggingOfHitLines)
            {
                LogHits(hitFiles, options.SearchToken);
            }

            WriteSummary(hitFiles, options.SearchToken);
        }

        private IEnumerable<FileHit> FindAllMatchingFiles()
        {
            string[] repos = _gitHubAdapter.FindRepositories();

            Console.WriteLine();
            Console.WriteLine($" Downloading files matching '{_configuration.FilenameFilter}' from {repos.Length} GitHub repositories owned by {_configuration.GithubTargetName}");

            return _gitHubAdapter.DownloadMatchingFiles(repos);
        }

        private FileHit[] FilterFilesWithSearchToken(IEnumerable<FileHit> files, string searchToken)
        {
            ConcurrentBag<FileHit> hits = new ConcurrentBag<FileHit>();

            ProgressIndicator.Start();

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 8 }, file =>
            {
                ProgressIndicator.SetTotal(_gitHubAdapter.CurrentSearchItemsCount);

                if (FindAllOccurences(file, searchToken))
                {
                    hits.Add(file);
                }

                ProgressIndicator.Step();
            });

            ProgressIndicator.WaitTillFinished();

            return hits.ToArray();
        }

        private bool FindAllOccurences(FileHit file, string searchToken)
        {
            int lineNr = 1;

            file.FoundLineNumbers = new List<int>();
            foreach (var line in file.Content.Split('\n'))
            {
                if (Regex.IsMatch(line, searchToken, RegexOptions.IgnoreCase))
                {
                    file.FoundLineNumbers.Add(lineNr);
                }

                lineNr++;
            }
            return file.FoundLineNumbers.Any();
        }

        private void LogHits(FileHit[] files, string searchToken)
        {
            if (!files.Any())
            {
                Console.WriteLine(" Search token has not been found in any repository.");
                return;
            }

            string lastRepo = string.Empty;
            foreach (var hit in files.OrderBy(x => x.RepositoryName))
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

        private void LogHit(FileHit hit, string searchToken)
        {
            LogHitFilename(hit);

            LogHitLines(hit, searchToken);
        }

        private void LogHitFilename(FileHit hit)
        {
            switch (_configuration.OutputMode)
            {
                case "HtmlUrl":
                    Console.WriteLine($" {hit.FoundLineNumbers.Count()} hits in {hit.HtmlUrl}");
                    break;
                default:
                    Console.WriteLine($" {hit.FoundLineNumbers.Count()} hits in {hit.Path}");
                    break;
            }
        }

        private void LogHitLines(FileHit hit, string searchToken)
        {
            var showLinesCount = _configuration.SurroundingLines;

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
                (value, c) => value + $"[{c.ToString().ToLower()}{c.ToString().ToUpper()}]");
        }

        private void WriteSummary(FileHit[] fileHits, string searchToken)
        {
            if (fileHits.Any())
            {
                string[] repositories = fileHits
                    .Select(x => x.RepositoryName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToArray();

                Console.WriteLine();
                Console.WriteLine($" Files matching '{_configuration.FilenameFilter}' and containing '{searchToken}' were found in the following {repositories.Length} repositories:");
                foreach (string batch in repositories)
                {
                    Console.WriteLine(" - " + batch);
                }
            }
        }
    }

    /// <summary>
    /// Performs the search on GitHub.
    /// </summary>
    internal interface IFileSearcher
    {
        /// <summary>
        /// Searches for the specified search token.
        /// </summary>
        void Search(Options options);
    }
}
