namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Describes a CSV header element.
    /// </summary>
    public interface ICsvHeader
    {
        /// <summary>
        /// Gets the index in which this header appears.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the title content of the header.
        /// </summary>
        string Title { get; }
    }
}
