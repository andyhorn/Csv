namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Describes an interface for storing a cell in a CSV table.
    /// </summary>
    public interface ICsvCell
    {
        /// <summary>
        /// Gets the value of the cell.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets the <see cref="ICsvRow"/> to which this cell belongs.
        /// </summary>
        ICsvRow Row { get; }

        /// <summary>
        /// Gets the <see cref="ICsvColumn"/> to which this cell belongs.
        /// </summary>
        ICsvColumn Column { get; }
    }
}
