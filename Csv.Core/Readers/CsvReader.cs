using System;
using System.IO;
using System.Linq;
using Csv.Core.Interfaces;

namespace Csv.Core.Readers
{
    /// <summary>
    /// Implements the <see cref="ICsvReader"/> interface.
    /// </summary>
    public class CsvReader : ICsvReader
    {
        private int _numColumns = 0;
        protected string[] _newLineOperators
            = new string[] { "\r", "\n", "\r\n" };

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        public CsvReader()
        {
            HasHeaders = true;
            Separator = ',';
        }

        /// <inheritdoc/>
        public bool HasHeaders { get; set; }

        /// <inheritdoc/>
        public char Separator { get; set; }

        /// <inheritdoc/>
        public virtual ICsv FromFile(string fileName)
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

            var csv = FromStream(stream);

            (csv as Models.Csv).Filename = fileName;

            return csv;
        }

        /// <inheritdoc/>
        public ICsv FromLines(string[] lines)
        {
            var csv = GetCsv();

            SetHeaders(csv, lines);
            SetNumColumns(lines);
            AddAllRows(csv, lines);

            return csv;
        }

        /// <inheritdoc/>
        public ICsv FromStream(Stream stream)
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Cannot read source.");
            }

            var csv = GetCsv();

            using var reader = new StreamReader(stream);

            SetHeaders(csv, reader);

            var currentLine = reader.ReadLine();
            SetNumColumns(currentLine);

            while (!string.IsNullOrWhiteSpace(currentLine))
            {
                AddRow(csv, currentLine);

                currentLine = reader.ReadLine();
            }

            return csv;
        }

        /// <inheritdoc/>
        public ICsv FromString(string csvData)
        {
            var lines = csvData.Split(_newLineOperators, StringSplitOptions.RemoveEmptyEntries);
            return FromLines(lines);
        }

        protected void AddAllRows(ICsv csv, string[] lines)
        {
            foreach (var line in lines)
            {
                AddRow(csv, line);
            }
        }

        protected void AddRow(ICsv csv, string line)
        {
            var rowData = line.Split(Separator);
            var rowIndex = csv.NumRows;

            for (var i = 0; i < _numColumns; i++)
            {
                csv.SetCell(rowIndex, i, rowData[i]);
            }
        }

        protected virtual ICsv GetCsv()
        {
            return new Models.Csv
            {
                Separator = Separator,
            };
        }

        protected virtual void SetHeaders(ICsv csv, string[] lines)
        {
            if (!HasHeaders)
            {
                return;
            }

            var headerValues = lines.First()
                .Split(Separator)
                .ToArray();
            var headers = new ICsvHeader[headerValues.Count()];

            for (var i = 0; i < headerValues.Count(); i++)
            {
                var newHeader = new Models.CsvHeader(i, headerValues[i]);
                headers[i] = newHeader;
            }

            csv.Headers = headers;
            lines = lines.Skip(1).ToArray();
        }

        protected virtual void SetHeaders(ICsv csv, StreamReader reader)
        {
            if (!HasHeaders)
            {
                return;
            }

            var headerLine = reader.ReadLine();
            SetHeaders(csv, new string[] { headerLine });
        }

        private void SetNumColumns(string[] lines)
        {
            SetNumColumns(lines.First());
        }

        private void SetNumColumns(string firstLine)
        {
            var numColumns = firstLine
                .Split(Separator)
                .Count();

            _numColumns = numColumns;
        }
    }
}