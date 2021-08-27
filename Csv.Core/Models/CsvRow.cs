using System.Runtime.CompilerServices;
using Csv.Core.Interfaces;

[assembly: InternalsVisibleTo("Csv.Test")]
namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsvRow"/> interface.
    /// </summary>
    internal class CsvRow : ICsvRow
    {
        private readonly ICsv _csv;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRow"/> class.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to which
        /// this row belongs.</param>
        /// <param name="index">The integer index at which this row appears.</param>
        public CsvRow(ICsv csv, int index)
        {
            _csv = csv;
            Index = index;
        }

        /// <inheritdoc/>
        public int Index { get; }

        /// <inheritdoc/>
        public ICsvCell[] Cells
        {
            get
            {
                var cells = new ICsvCell[_csv.NumColumns];

                for (var i = 0; i < _csv.NumColumns; i++)
                {
                    cells[i] = _csv.Cells[Index][i];
                }

                return cells;
            }
        }
    }
}
