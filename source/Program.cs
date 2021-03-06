﻿using System;
using System.Reflection;

using CommandLine;

using GitHubSearch.Common;

namespace GitHubSearch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Options options = GetCommandLineOptions(args);

            PerformSearch(options);
        }

        private static Options GetCommandLineOptions(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArgumentsStrict(args, options);
            if (!options.IsValid)
            {
                Console.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }

            return options;
        }

        private static void PerformSearch(Options options)
        {
            try
            {
                var container = Bootstrapper.Start(options);

                var searcher = container.Resolve<IFileSearcher>();
                searcher.Search();
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
    }
}