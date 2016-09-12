using System.Collections.Generic;
using System.Linq;

namespace GitHubSearch.Common
{
    internal static class StringExtensionMethods
    {
        public static IEnumerable<string> BatchesOfMaxLength(this IEnumerable<string> source, int maxStringSize)
        {
            var batch = new List<string>();

            foreach (var value in source)
            {
                if (batch.Sum(x => x.Length) + value.Length > maxStringSize)
                {
                    yield return string.Join(", ", batch);
                    batch = new List<string>();
                }
                batch.Add(value);
            }

            if (batch.Any())
            {
                yield return string.Join(", ", batch);
            }
        }
    }
}
