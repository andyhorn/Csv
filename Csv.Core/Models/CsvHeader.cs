using System.Runtime.CompilerServices;
using Csv.Core.Interfaces;

[assembly: InternalsVisibleTo("Csv.Test")]
namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsvHeader"/> interface.
    /// </summary>
    internal class CsvHeader : ICsvHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ICsvHeader"/> class.
        /// </summary>
        /// <param name="index">The index at which this header appears.</param>
        /// <param name="title">The title/content of this header cell.</param>
        public CsvHeader(int index, string title)
        {
            Index = index;
            Title = title;
        }

        /// <inheritdoc/>
        public int Index { get; }

        /// <inheritdoc/>
        public string Title { get; }
    }
}
