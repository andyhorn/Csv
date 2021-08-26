using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv.Core.Interfaces;
using Csv.Core.Models;

namespace Csv.Core
{
    public static class CsvReader
    {
        private static string[] NewLineOperators = new string[] { "\r", "\n", "\r\n" };

        public static ICsv FromString(string csvData, bool hasHeaders = true, char separator = ',')
        {
            var lines = csvData.Split(NewLineOperators, StringSplitOptions.RemoveEmptyEntries);
            return FromLines(lines, hasHeaders, separator);
        }

        public static ICsv FromLines(string[] lines, bool hasHeaders = true, char separator = ',')
        {
            var csv = new Models.Csv
            {
                HasHeaders = hasHeaders,
                Separator = separator,
            };

            var numColumns = AddHeaders(csv, lines[0]);
            AddColumns(csv, numColumns);
            lines = hasHeaders ? lines.Skip(1).ToArray() : lines;

            for (var i = 0; i < lines.Count(); i++)
            {
                AddRowData(csv, lines[i], numColumns);
            }

            return csv;
        }

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

        public static ICsv FromStream(Stream stream, bool hasHeaders = true, char separator = ',')
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Cannot read csv file.");
            }

            var csv = new Models.Csv
            {
                HasHeaders = hasHeaders,
                Separator = separator,
            };

            using var reader = new StreamReader(stream);
            var currentLine = reader.ReadLine(); // read the first line

            var numColumns = AddHeaders(csv, currentLine);
            AddColumns(csv, numColumns);
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
            var row = new CsvRow(csv.Rows.Count());

            for (var i = 0; i < numColumns; i++)
            {
                if (i >= rowData.Length)
                {
                    break;
                }

                var cell = new CsvCell(rowData[i])
                {
                    Row = row,
                    Column = csv.Columns.ElementAt(i),
                };

                AddToRow(row, cell);
                AddToColumn(csv, i, cell);
            }

            AddToCsv(csv, row);
        }

        private static int AddHeaders(ICsv csv, string firstLine)
        {
            if (!csv.HasHeaders)
            {
                return firstLine.Split(csv.Separator).Count();
            }

            var headers = firstLine.Split(csv.Separator)
                .Select((h, i) => new CsvHeader(i, h))
                .ToList();

            (csv as Models.Csv).Headers = headers;

            return csv.Headers.Count();
        }

        private static void AddColumns(ICsv csv, int numColumns)
        {
            for (var i = 0; i < numColumns; i++)
            {
                var header = csv.HasHeaders && csv.Headers.Count() > i
                    ? csv.Headers.ElementAt(i) : null;
                var column = new CsvColumn(i, header);

                ((List<ICsvColumn>)csv.Columns).Add(column);
            }
        }

        private static void AddToRow(ICsv csv, int rowIndex, ICsvCell cell)
        {
            AddToRow(csv.Rows.ElementAt(rowIndex), cell);
        }

        private static void AddToRow(ICsvRow row, ICsvCell cell)
        {
            ((List<ICsvCell>)row.Cells).Add(cell);
        }

        private static void AddToColumn(ICsv csv, int columnIndex, ICsvCell cell)
        {
            ((List<ICsvCell>)csv.Columns.ElementAt(columnIndex).Cells).Add(cell);
        }

        private static void AddToCsv(ICsv csv, ICsvRow row)
        {
            ((List<ICsvRow>)csv.Rows).Add(row);
        }
    }
}
