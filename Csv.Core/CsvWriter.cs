using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Csv.Core.Interfaces;

namespace Csv.Core
{
    /// <summary>
    /// Utility class that provides methods for writing an <see cref="ICsv"/> instance.
    /// </summary>
    public static class CsvWriter
    {
        /// <summary>
        /// Writes the contents of an <see cref="ICsv"/> instance to a local file asynchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to be written.</param>
        /// <param name="filePath">The path at which to write the file data.</param>
        public static async Task ToFileAsync(ICsv csv, string filePath)
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

        /// <summary>
        /// Writes an <see cref="ICsv"/> instance to a local file.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to be written.</param>
        /// <param name="fileName">The path at which to write the file data.</param>
        public static void ToFile(ICsv csv, string fileName)
        {
            ToFileAsync(csv, fileName).GetAwaiter();
        }

        /// <summary>
        /// Writes an <see cref="ICsv"/> instance to a supplied <see cref="Stream"/>.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to be written.</param>
        /// <param name="stream">The <see cref="Stream"/> instance to be written to.</param>
        public static async Task ToStream(ICsv csv, Stream stream)
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

        private static async Task WriteToWriterAsync(ICsv csv, StreamWriter writer)
        {
            foreach (var row in csv.Rows)
            {
                await writer.WriteLineAsync(string.Join(csv.Separator, row.Cells.Select(c => c.Value)));
            }
        }
    }
}
