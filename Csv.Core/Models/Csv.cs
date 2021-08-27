using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Csv.Core.Interfaces;

[assembly: InternalsVisibleTo("Csv.Test")]
namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsv"/> interface.
    /// </summary>
    internal class Csv : ICsv
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Csv"/> class.
        /// </summary>
        public Csv()
        {
            Cells = new ICsvCell[0][];
            Headers = new ICsvHeader[0];
        }

        /// <inheritdoc/>
        public string Filename { get; set; }

        /// <inheritdoc/>
        public bool HasHeaders => Headers != null && Headers.Any();

        /// <inheritdoc/>
        public char Separator { get; set; }

        /// <inheritdoc/>
        public int NumRows => Cells.Count();

        /// <inheritdoc/>
        public int NumColumns => Cells.Any() ? Cells[0].Count() : Headers.Count();

        /// <inheritdoc/>
        public ICsvHeader[] Headers { get; set; }

        /// <inheritdoc/>
        public ICsvRow[] Rows => Cells.Select((x, i) => new CsvRow(this, i)).ToArray();

        /// <inheritdoc/>
        public ICsvColumn[] Columns => Cells.Any() ? Cells[0]
            .Select((x, i) => new CsvColumn(this, i, HasHeaders && i < Headers.Count()
                ? Headers[i] : null))
            .ToArray() : new ICsvColumn[0];

        /// <inheritdoc/>
        public ICsvCell[][] Cells { get; internal set; }

        /// <inheritdoc/>
        public ICsvCell GetCell(int row, int col)
        {
            if (!Cells.Any() || !Cells.First().Any())
            {
                return null;
            }

            if (row < 0 || row >= Cells.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (col < 0 || col >= Cells.First().Count())
            {
                throw new ArgumentOutOfRangeException(nameof(col));
            }

            return Cells[row][col];
        }

        /// <inheritdoc/>
        public void SetCell(int row, int col, object value)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (col < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(col));
            }

            var addedNewCells = false;

            if (row >= NumRows)
            {
                var difference = (row + 1) - NumRows;
                AddNewRows(difference);
                addedNewCells = true;
            }

            if (col >= NumColumns)
            {
                var difference = (col + 1) - NumColumns;
                AddNewColumns(difference);
                addedNewCells = true;
            }

            if (addedNewCells)
            {
                FillEmptyCells();
            }

            Cells[row][col] = new CsvCell(this, row, col, value);
        }

        private void FillEmptyCells()
        {
            for (var rowIndex = 0; rowIndex < NumRows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < NumColumns; columnIndex++)
                {
                    if (Cells[rowIndex][columnIndex] == null)
                    {
                        Cells[rowIndex][columnIndex] = new CsvCell(this, rowIndex, columnIndex, null);
                    }
                }
            }
        }

        private void AddNewRows(int numRows)
        {
            for (var i = 0; i < numRows; i++)
            {
                var newRow = new ICsvCell[NumColumns];
                Cells = Cells.Append(newRow).ToArray();
            }
        }

        private void AddNewColumns(int numColumns)
        {
            for (var i = 0; i < NumRows; i++)
            {
                var newRow = Cells[i].ToList();
                newRow.AddRange(new ICsvCell[numColumns]);

                Cells[i] = newRow.ToArray();
            }
        }
    }
}
