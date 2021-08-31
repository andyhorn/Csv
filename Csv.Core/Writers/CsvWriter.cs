using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csv.Core.Interfaces;

namespace Csv.Core.Writers
{
    /// <summary>
    /// Implements the <see cref="ICsvWriter"/> interface.
    /// </summary>
    public class CsvWriter : ICsvWriter
    {
        /// <inheritdoc/>
        public async Task ToFileAsync(ICsv csv, string filePath)
        {
            if (csv == null)
            {
                throw new ArgumentNullException(nameof(csv));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            filePath = Path.GetFullPath(filePath);

            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException();
            }

            using var stream = File.OpenWrite(filePath);
            using var writer = new StreamWriter(stream);

            await WriteToWriterAsync(csv, writer);
        }

        /// <inheritdoc/>
        public void ToFile(ICsv csv, string filePath)
        {
            ToFileAsync(csv, filePath).GetAwaiter();
        }

        /// <inheritdoc/>
        public async Task ToStreamAsync(ICsv csv, Stream stream)
        {
            if (csv == null)
            {
                throw new ArgumentNullException(nameof(csv));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer = new StreamWriter(stream);
            await WriteToWriterAsync(csv, writer);
        }

        /// <inheritdoc/>
        public void ToStream(ICsv csv, Stream stream)
        {
            ToStreamAsync(csv, stream).GetAwaiter();
        }

        /// <inheritdoc/>
        public Task<string> ToStringAsync(ICsv csv)
        {
            var builder = new StringBuilder();

            if (csv.HasHeaders)
            {
                var headers = GetHeaders(csv);
                var headerLine = string.Join(csv.Separator, headers);
                builder.AppendLine(headerLine);
            }

            foreach (var row in csv.Rows)
            {
                var rowValues = row.Cells.Select(c => c.Value);
                var rowLine = string.Join(csv.Separator, rowValues);
                builder.AppendLine(rowLine);
            }

            return Task.FromResult(builder.ToString());
        }

        /// <inheritdoc/>
        public string ToString(ICsv csv)
        {
            return ToStringAsync(csv).GetAwaiter().GetResult();
        }

        private async Task WriteToWriterAsync(ICsv csv, StreamWriter writer)
        {
            if (csv.HasHeaders)
            {
                var headerValues = GetHeaders(csv);
                var headerLine = string.Join(csv.Separator, headerValues);
                await writer.WriteLineAsync(headerLine);
            }

            foreach (var row in csv.Rows)
            {
                var rowValues = row.Cells.Select(c => c.Value);
                var rowLine = string.Join(csv.Separator, rowValues);
                await writer.WriteLineAsync(rowLine);
            }
        }

        protected virtual string[] GetHeaders(ICsv csv)
        {
            var headerValues = csv.Headers.Select(h => h.Title);
            return headerValues.ToArray();
        }
    }
}
