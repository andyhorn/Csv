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
        /// <returns>A new instance of the <see cref="ICsv"/> interface.</returns>
        public static ICsv New()
            => new Models.Csv();

        /// <summary>
        /// Gets a new instance of the <see cref="ICsv{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">The class <see cref="Type"/> parameter
        /// for the CSV object.</typeparam>
        /// <returns>A new instance of the <see cref="ICsv{T}"/> interface.</returns>
        public static ICsv<T> ForType<T>()
            where T : class
            => new Models.Csv<T>();
    }
}
