using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity_Tools
{
    public static class CsvHelper
    {
        /// <summary>
        /// Generates a csv string from a set of objects using given column definitions.
        /// </summary>
        /// <typeparam name="T">The type of the items to parse.</typeparam>
        /// <param name="items">The items to parse.</param>
        /// <param name="columnDefinitions">The definition on how to generate the csv text.</param>
        /// <exception cref="ArgumentNullException"><see cref="items"/> and <see cref="columnDefinitions"/> must not be null.</exception>
        /// <returns>Returns the generated csv text.</returns>
        public static string ToCsv<T>(this IEnumerable<T> items, params (string title, Func<T, string> valueProvider)[] columnDefinitions)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (columnDefinitions == null)
            {
                throw new ArgumentNullException(nameof(columnDefinitions));
            }

            if (columnDefinitions.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            foreach (var cd in columnDefinitions)
            {
                builder.AddCsvValue(cd.title);
                builder.Append(',');
            }

            builder.AppendLine();

            foreach (var item in items)
            {
                foreach (var cd in columnDefinitions)
                {
                    builder.AddCsvValue(cd.valueProvider(item));
                    builder.Append(',');
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the string value to a string builder using common CSV rules.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="value">The string value.</param>
        public static void AddCsvValue(this StringBuilder builder, string value)
        {
            var containsEscapeChar = value.Contains(',');
            if (containsEscapeChar)
            {
                builder.Append('"');
            }

            builder.EnsureCapacity(builder.Length + value.Length);

            foreach (var c in value)
            {
                builder.Append(c);

                if (c == '"')
                {
                    builder.Append('"');
                }
            }

            if (containsEscapeChar)
            {
                builder.Append('"');
            }
        }

        public static string GetCsvValue(string value)
        {
            value = value.Replace("\"", "\"\"");
            return value.Contains(",") ? $"\"{value}\"" : value;
        }
    }
}
