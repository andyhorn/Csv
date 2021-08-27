using System.Linq;
using System.Runtime.CompilerServices;
using Csv.Core.Interfaces;

[assembly: InternalsVisibleTo("Csv.Test")]
namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsvCell"/> interface.
    /// </summary>
    internal class CsvCell : ICsvCell
    {
        private readonly ICsv _csv;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvCell"/> class.
        /// </summary>
        /// <param name="csv">The <see cref="ICsv"/> instance to which
        /// this cell belongs.</param>
        /// <param name="row">The integer row index of this cell.</param>
        /// <param name="column">The integer column index of this cell.</param>
        /// <param name="value">The content/value of this cell.</param>
        public CsvCell(ICsv csv, int row, int column, object value)
        {
            _csv = csv;
            RowIndex = row;
            ColumnIndex = column;

            Value = value;
        }

        /// <inheritdoc/>
        public object Value { get; }

        /// <inheritdoc/>
        public int RowIndex { get; }

        /// <inheritdoc/>
        public int ColumnIndex { get; }

        /// <inheritdoc/>
        public ICsvRow Row => _csv.Rows.ElementAt(RowIndex);

        /// <inheritdoc/>
        public ICsvColumn Column => _csv.Columns.ElementAt(ColumnIndex);
    }
}
