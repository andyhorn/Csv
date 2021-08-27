namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Describes a column within a CSV table.
    /// </summary>
    public interface ICsvColumn
    {
        /// <summary>
        /// Gets the index of this column.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets an array of <see cref="ICsvCell"/> that belong to this column,
        /// ordered by the row in which they appear.
        /// </summary>
        ICsvCell[] Cells { get; }

        /// <summary>
        /// Gets the <see cref="ICsvHeader"/> for this column.
        /// </summary>
        ICsvHeader Header { get; }
    }
}
