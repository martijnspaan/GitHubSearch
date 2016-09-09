using System;
using System.Reflection;
using GitHubSearch.Common;

namespace GitHubSearch
{
    internal class Program
    {
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

            var container = Bootstrapper.Start();

            try
            {
                var searcher = container.Resolve<IConfigurationSearcher>();
                searcher.SearchFor(args[0]);
            }
            catch (AggregateException aggregateException)
            {
                Console.WriteLine();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    Console.WriteLine(innerException.Message);
                }
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
                var version = Assembly.GetCallingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Revision}";
            }
        }
    }
}