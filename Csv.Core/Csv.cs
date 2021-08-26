using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv.Core.Interfaces;
using Csv.Core.Models;

namespace Csv.Core
{
    public class Csv : ICsv
    {
        private Csv()
        {
            Headers = new List<ICsvHeader>();
            Rows = new List<ICsvRow>();
            Columns = new List<ICsvColumn>();
        }

        public bool HasHeaders { get; private set; }

        public char Separator { get; private set; }

        public string Filename { get; private set; }

        public IEnumerable<ICsvHeader> Headers { get; private set; }

        public IEnumerable<ICsvRow> Rows { get; }

        public IEnumerable<ICsvColumn> Columns { get; }

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
            (csv as Csv).Filename = fileName;

            return csv;
        }

        public static ICsv FromStream(Stream stream, bool hasHeaders = true, char separator = ',')
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Cannot read csv file.");
            }

            var csv = new Csv
            {
                HasHeaders = hasHeaders,
                Separator = separator,
            };

            using var reader = new StreamReader(stream);
            var currentLine = reader.ReadLine(); // read the first line

            if (hasHeaders)
            {
                var headerValues = currentLine.Split(separator);
                var headers = headerValues
                    .Select((x, i) => new CsvHeader(i, x))
                    .ToList();

                csv.Headers = headers;
                currentLine = reader.ReadLine();
            }

            var numColumns = currentLine.Count(x => x.Equals(separator));
            var rowIndex = 0;

            for (var i = 0; i < numColumns; i++)
            {
                var header = hasHeaders ? csv.Headers.ElementAt(i) : null;
                ((List<ICsvColumn>)csv.Columns).Add(new CsvColumn(i, header));
            }

            while (!string.IsNullOrEmpty(currentLine))
            {
                var row = new CsvRow(rowIndex++);

                var rowData = currentLine.Split(separator);

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

                    ((List<ICsvCell>)row.Cells).Add(cell);
                    ((List<ICsvCell>)csv.Columns.ElementAt(i)).Add(cell);
                }
            }

            return csv;
        }
    }
}
