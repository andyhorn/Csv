using System;
using System.IO;
using System.Linq;
using Csv.Core.Interfaces;
using Csv.Core.Models;

namespace Csv.Core
{
    /// <summary>
    /// Utility class that provides methods for reading raw data and
    /// constructing <see cref="ICsv"/> objects.
    /// </summary>
    public static class CsvReader
    {
        private static string[] NewLineOperators = new string[] { "\r", "\n", "\r\n" };

        /// <summary>
        /// Creates an <see cref="ICsv"/> instance from a CSV-formatted string.
        /// </summary>
        /// <param name="csvData">A string containing CSV-formatted data.</param>
        /// <param name="hasHeaders">A value that indicates whether the
        /// incoming data contains a row of headers.</param>
        /// <param name="separator">The character that separates individual cells.</param>
        /// <returns>A new <see cref="ICsv"/> instance containing the CSV data.</returns>
        public static ICsv FromString(string csvData, bool hasHeaders = true, char separator = ',')
        {
            var lines = csvData.Split(NewLineOperators, StringSplitOptions.RemoveEmptyEntries);
            return FromLines(lines, hasHeaders, separator);
        }

        /// <summary>
        /// Creates an <see cref="ICsv"/> instance from a collection of CSV-formatted
        /// strings, with each string representing a row of data.
        /// </summary>
        /// <param name="lines">A collection of CSV-formatted strings.</param>
        /// <param name="hasHeaders">A value indicating whether the first row
        /// of data contains headers.</param>
        /// <param name="separator">The character that separates individuals cells.</param>
        /// <returns>A new <see cref="ICsv"/> instance containing the CSV data.</returns>
        public static ICsv FromLines(string[] lines, bool hasHeaders = true, char separator = ',')
        {
            var csv = new Models.Csv
            {
                HasHeaders = hasHeaders,
                Separator = separator,
            };

            var numColumns = AddHeaders(csv, lines[0]);
            lines = hasHeaders ? lines.Skip(1).ToArray() : lines;

            for (var i = 0; i < lines.Count(); i++)
            {
                AddRowData(csv, lines[i], numColumns);
            }

            return csv;
        }

        /// <summary>
        /// Creates an <see cref="ICsv"/> instance by reading CSV-formatted data
        /// from a local file.
        /// </summary>
        /// <param name="fileName">The path of the file to be read and parsed.</param>
        /// <param name="hasHeaders">A value indicating whether the first row of
        /// data contains headers.</param>
        /// <param name="separator">The character that separates individual cells.</param>
        /// <returns>A new <see cref="ICsv"/> instance containing the CSV data.</returns>
        public static ICsv FromFile(string fileName, bool hasHeaders = true, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            using var stream = File.OpenRead(fileName);
            var csv = FromStream(stream, hasHeaders, separator);
            (csv as Models.Csv).Filename = fileName;

            return csv;
        }

        /// <summary>
        /// Creates an <see cref="ICsv"/> instance from a <see cref="Stream"/> of
        /// CSV-formatted data.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> instance to be read.</param>
        /// <param name="hasHeaders">A value indicating whether the first row
        /// of data contains headers.</param>
        /// <param name="separator">The character that separates individual cells.</param>
        /// <returns>A new <see cref="ICsv"/> instance containing the CSV data.</returns>
        public static ICsv FromStream(Stream stream, bool hasHeaders = true, char separator = ',')
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Cannot read from stream.");
            }

            var csv = new Models.Csv
            {
                HasHeaders = hasHeaders,
                Separator = separator,
            };

            using var reader = new StreamReader(stream);
            var currentLine = reader.ReadLine(); // read the first line

            var numColumns = AddHeaders(csv, currentLine);
            currentLine = hasHeaders ? reader.ReadLine() : currentLine;

            while (!string.IsNullOrEmpty(currentLine))
            {
                AddRowData(csv, currentLine, numColumns);
                currentLine = reader.ReadLine();
            }

            return csv;
        }

        private static void AddRowData(ICsv csv, string rowString, int numColumns)
        {
            var rowData = rowString.Split(csv.Separator);
            var rowIndex = csv.Rows.Count();

            for (var i = 0; i < numColumns; i++)
            {
                csv.SetCell(rowIndex, i, rowData[i]);
            }
        }

        private static int AddHeaders(ICsv csv, string firstLine)
        {
            var count = firstLine.Split(csv.Separator).Count();

            if (!csv.HasHeaders)
            {
                return count;
            }

            var headers = firstLine.Split(csv.Separator)
                .Select((h, i) => new CsvHeader(i, h))
                .ToList();

            csv.Headers = headers.ToArray();

            return count;
        }
    }
}
