using System.Reflection;

using CommandLine;
using CommandLine.Text;

namespace GitHubSearch
{
    internal class Options
    {
        [Option('q', "quiet", Required = false, DefaultValue = false, HelpText = "Suppresses the logging of hit lines.")]
        public bool SuppressLoggingOfHitLines { get; set; }

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