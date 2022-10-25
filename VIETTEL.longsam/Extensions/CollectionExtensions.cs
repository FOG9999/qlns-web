using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Viettel
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        public static List<List<T>> ChunkByChar<T>(this List<T> source, int chunkSize = 50)
        {
            List<List<T>> lines = new List<List<T>>();
            List<T> words = new List<T>();
            int count = 0;
            if (chunkSize == 100)
                chunkSize = 112;
            for (int i = 0; i < source.Count; i++)
            {
                count += source[i].ToString().Length;
                if (count >= chunkSize)
                {
                    if (count == chunkSize)
                    {
                        words.Add(source[i]);
                        count = 0;
                        lines.Add(words);
                        words = new List<T>();
                    }
                    else
                    {
                        count = source[i].ToString().Length;
                        lines.Add(words);
                        words = new List<T>();
                        words.Add(source[i]);
                    }
                }
                else if (count == chunkSize)
                {
                    words.Add(source[i]);
                    lines.Add(words);
                    count = 0;
                    words = new List<T>();
                }
                else
                {
                    words.Add(source[i]);
                    count += 1;
                }                
            }
            if (count <= chunkSize && words.Count > 0)
            {
                lines.Add(words);
            }

            return lines;
        }
        public static List<string> SplitOn(this string initial, int MaxCharacters)
        {

            List<string> lines = new List<string>();

            if (string.IsNullOrEmpty(initial) == false)
            {
                string targetGroup = "Line";
                string pattern = string.Format(@"(?<{0}>.{{1,{1}}})(?:\W|$)", targetGroup, MaxCharacters);

                var t1 = Regex.Matches(initial, pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant);
                var t2 = t1.OfType<Match>();
                var t3 = t2.Select(mt => mt.Groups[targetGroup].Value);

                lines = Regex.Matches(initial, pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant)
                   .OfType<Match>()
                    .Select(mt => mt.Groups[targetGroup].Value)
                    .ToList();
            }
            return lines;
        }
    }
}
