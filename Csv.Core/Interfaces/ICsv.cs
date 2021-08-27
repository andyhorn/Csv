namespace Csv.Core.Interfaces
{
    public interface ICsv
    {
        /// <summary>
        /// Gets or sets the absolute filename of the CSV file, if one was used.
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// Gets a value indicating whether this CSV has a header row.
        /// </summary>
        bool HasHeaders { get; }

        /// <summary>
        /// Gets or sets the separator used to separate cells.
        /// </summary>
        char Separator { get; set; }

        /// <summary>
        /// Gets the current number of rows.
        /// </summary>
        int NumRows { get; }

        /// <summary>
        /// Gets the current number of columns.
        /// </summary>
        int NumColumns { get; }

        /// <summary>
        /// Gets or sets an array of <see cref="ICsvHeader"/>.
        /// </summary>
        ICsvHeader[] Headers { get; set; }

        /// <summary>
        /// Gets an array of <see cref="ICsvRow"/>.
        /// </summary>
        ICsvRow[] Rows { get; }

        /// <summary>
        /// Gets an array of <see cref="ICsvColumn"/>.
        /// </summary>
        ICsvColumn[] Columns { get; }

        /// <summary>
        /// Gets all <see cref="ICsvCell"/> objects in row/column order.
        /// </summary>
        ICsvCell[][] Cells { get; }

        /// <summary>
        /// Gets a <see cref="ICsvCell"/> at a specific row/column index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        /// <returns>A single <see cref="ICsvCell"/> object.</returns>
        ICsvCell GetCell(int row, int col);

        /// <summary>
        /// Creates a new <see cref="ICsvCell"/> object at the specified
        /// row/column index with the given value.
        ///
        /// This call will create additional rows and/or columns, as needed.
        /// </summary>
        /// <param name="row">The row index for the new cell.</param>
        /// <param name="col">The column index for the new cell.</param>
        /// <param name="value">The value to be set in the new cell.</param>
        void SetCell(int row, int col, object value);
    }
}
