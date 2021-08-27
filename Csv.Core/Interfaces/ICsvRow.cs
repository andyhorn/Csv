namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Describes a row of cells within a CSV table.
    /// </summary>
    public interface ICsvRow
    {
        /// <summary>
        /// Gets the index at which this row appears.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets a list of <see cref="ICsvCell"/> that belong to this row,
        /// ordered by the column in which they appear.
        /// </summary>
        ICsvCell[] Cells { get; }
    }
}
