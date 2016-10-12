using System.Reflection;

using CommandLine;
using CommandLine.Text;

namespace GitHubSearch
{
    internal class Options
    {
        [Option('q', "quiet", Required = false, DefaultValue = false, HelpText = "Suppresses the logging of hit lines.")]
        public bool SuppressLoggingOfHitLines { get; set; }

        [Option('t', "target", Required = false, DefaultValue = null, HelpText = "The name of either organization or user containing the repositories to search through. This will override the 'GithubTargetName' value in the configuration file.")]
        public string GithubTargetName { get; set; }

        [Option('r', "repositories", Required = false, DefaultValue = null, HelpText = "The filter for repositories to search through. This will override the 'RepositoryFilters' value in the configuration file.")]
        public string RepositoryFilters { get; set; }

        [Option('f', "filename", Required = false, DefaultValue = null, HelpText = "The filter for filenames to search for. Bound to the search rules of GitHub search. This will override the 'FilenameFilter' value in the configuration file.")]
        public string FilenameFilter { get; set; }

        [Option('o', "output", Required = false, DefaultValue = null, HelpText = "Specifies how the filename is shown in the output. Options: Path, HtmlUrl. This will override the 'OutputMode' value in the configuration file.")]
        public string OutputMode { get; set; }

        [Option('l', "lines", Required = false, DefaultValue = null, HelpText = "Specifies the amount of lines shown above and below the highlighted line. This will override the 'SurroundingLines' value in the configuration file.")]
        public int? SurroundingLines { get; set; }

        [Option('c', "flushcache", Required = false, DefaultValue = false, HelpText = "Empties the cache folder, forcing downloads of online source.")]
        public bool FlushCache { get; set; }

        [ValueOption(0)]
        public string SearchToken { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(SearchToken);

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo($"GitHubSearch {ProductVersion}"),
                Copyright = CopyrightInfo.Default,
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine(@"Usage: GitHubSearch.exe <searchtoken> [options]");
            help.AddOptions(this);

            return help;
        }

        internal static string ProductVersion
        {
            get
            {
                var version = Assembly.GetCallingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }
    }
}