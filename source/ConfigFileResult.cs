using System;
using System.Collections.Generic;

namespace GitHubSearch
{
    /// <summary>
    /// Contains details about the hit configuration file.
    /// </summary>
    public class ConfigFileHit
    {
        /// <summary>
        /// The contents of the configuration file.
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The repository name that contains the configuration file.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// The url to the configuration file.
        /// </summary>
        public Uri HtmlUrl { get; set; }

        /// <summary>
        /// The relative path to the configuration file within the repository.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The line numbers that are hit for the search token.
        /// </summary>
        public List<int> FoundLineNumbers { get; set; }
    }
}
