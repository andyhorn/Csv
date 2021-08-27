using Csv.Core.Interfaces;

namespace Csv.Core
{
    /// <summary>
    /// Factory class for creating new instances of the <see cref="ICsv"/> interface.
    /// </summary>
    public static class CsvFactory
    {
        /// <summary>
        /// Gets a new instance of the <see cref="ICsv"/> interface.
        /// </summary>
        public static ICsv New { get => new Models.Csv(); }
    }
}
