using System.IO;
using System.Threading.Tasks;

namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Defines an interface for writing a <see cref="ICsv"/> object.
    /// </summary>
    public interface ICsvWriter
    {
        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to
        /// file, asynchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance
        /// containing the data to be written.</param>
        /// <param name="filePath">The file path at which to write
        /// the data.</param>
        Task ToFileAsync(ICsv csv, string filePath);

        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to
        /// file, synchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance
        /// containing the data to be written.</param>
        /// <param name="filePath">The file path at which to write
        /// the data.</param>
        void ToFile(ICsv csv, string filePath);

        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to a
        /// <see cref="Stream"/> object, asynchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance
        /// containing the data to be written.</param>
        /// <param name="stream">The <see cref="Stream"/> object
        /// to be written to.</param>
        Task ToStreamAsync(ICsv csv, Stream stream);

        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to a
        /// <see cref="Stream"/> object, synchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance
        /// containing the data to be written.</param>
        /// <param name="stream">The <see cref="Stream"/> object
        /// to be written to.</param>
        void ToStream(ICsv csv, Stream stream);

        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to a
        /// CSV-formatted string, asynchronously
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> object
        /// containing the data to be written.</param>
        /// <returns>A string containing the CSV-formatted data.</returns>
        Task<string> ToStringAsync(ICsv csv);

        /// <summary>
        /// Writes a <see cref="ICsv"/> object's data to a
        /// CSV-formatted string, synchronously.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> object
        /// containing the data to be written.</param>
        /// <returns>A string containing the CSV-formatted data.</returns>
        string ToString(ICsv csv);
    }
}