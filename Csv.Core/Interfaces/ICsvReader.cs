using System.IO;

namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Defines an interface for reading CSV-formatted
    /// data and producing an <see cref="ICsv"/> instance.
    /// </summary>
    public interface ICsvReader
    {
        /// <summary>
        /// Gets or sets a value indicating whether the
        /// CSV-formatted data contains a row of headers.
        /// </summary>
        bool HasHeaders { get; set; }

        /// <summary>
        /// Gets or sets the value that separates individual
        /// cells in the CSV-formatted data.
        /// </summary>
        char Separator { get; set; }

        /// <summary>
        /// Parses a string of CSV-formatted data into a new
        /// <see cref="ICsv"/> instance.
        /// </summary>
        /// <param name="csvData">A string containing
        /// CSV-formatted data.</param>
        /// <returns>A new <see cref="ICsv"/> instance.</returns>
        ICsv FromString(string csvData);

        /// <summary>
        /// Parses a collection of strings containing CSV-formatted
        /// data into a new <see cref="ICsv"/> instance.
        /// </summary>
        /// <param name="lines">An array of strings containing
        /// CSV-formatted data.</param>
        /// <returns>A new <see cref="ICsv"/> instance.</returns>
        ICsv FromLines(string[] lines);

        /// <summary>
        /// Reads and parses a CSV-formatted file to produce a
        /// new <see cref="ICsv"/> instance.
        /// </summary>
        /// <param name="fileName">The filename for a CSV file.</param>
        /// <returns>A new <see cref="ICsv"/> instance.</returns>
        ICsv FromFile(string fileName);

        /// <summary>
        /// Reads CSV-formatted data from a <see cref="Stream"/> source
        /// and produces a new <see cref="ICsv"/> instance.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> object with
        /// CSV-formatted data to be read.</param>
        /// <returns>A new <see cref="ICsv"/> instance.</returns>
        ICsv FromStream(Stream stream);
    }
}
