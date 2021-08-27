using System.Runtime.CompilerServices;
using Csv.Core.Interfaces;

[assembly: InternalsVisibleTo("Csv.Test")]
namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsvColumn"/> interface.
    /// </summary>
    internal class CsvColumn : ICsvColumn
    {
        private readonly ICsv _csv;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn"/> class.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to which
        /// this column belongs.</param>
        /// <param name="index">The integer index of this column.</param>
        /// <param name="header">The <see cref="ICsvHeader"/> that
        /// describes this column.</param>
        public CsvColumn(ICsv csv, int index, ICsvHeader header)
        {
            _csv = csv;
            Index = index;
            Header = header;
        }

        /// <inheritdoc/>
        public int Index { get; }

        /// <inheritdoc/>
        public ICsvCell[] Cells
        {
            get
            {
                var cells = new ICsvCell[_csv.NumRows];

                for (var i = 0; i < _csv.NumRows; i++)
                {
                    cells[i] = _csv.Cells[i][Index];
                }

                return cells;
            }
        }

        /// <inheritdoc/>
        public ICsvHeader Header { get; internal set; }
    }
}
