using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicketSystem.DiscordBot.Helpers
{
    public static class StringHelper
    {
        public static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }
            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            var lengthA = a.Length;
            var lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (var i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (var j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (var i = 1; i <= lengthA; i++)
            for (var j = 1; j <= lengthB; j++)
            {
                var cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Math.Min
                (
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                );
            }
            return distances[lengthA, lengthB];
        }

        public static T? FindBestMatch<T>(this IEnumerable<T> enumerable, Func<T, string> termSelector, string searchString)
        {
            return enumerable.Where(x =>
                    termSelector(x).IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                .MinBy(asset =>
                    LevenshteinDistance(searchString, termSelector(asset)));
        }

        public static string JoinRows(int spacing, params string[][] rows)
        {
            var numCols = rows.Max(x => x.Length);

            var maxColLens = new int[numCols];

            foreach (var row in rows)
            {
                for (var j = 0; j < numCols; j++)
                {
                    if (row.Length <= j)
                        break;

                    if (row[j].Length > maxColLens[j])
                        maxColLens[j] = row[j].Length;
                }
            }

            var builder = new StringBuilder();

            foreach (var row in rows)
            {
                for (var j = 0; j < numCols; j++)
                {
                    var spaces = maxColLens[j] + (j == numCols - 1 ? 0 : spacing);

                    if (row.Length > j)
                    {
                        builder.Append(row[j]);
                        spaces -= row[j].Length;
                    }

                    builder.Append(new string(' ', spaces));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
