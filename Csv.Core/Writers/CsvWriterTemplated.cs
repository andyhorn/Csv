using System.Linq;
using System.Reflection;
using Csv.Core.Interfaces;

namespace Csv.Core.Writers
{
    /// <summary>
    /// Implements the <see cref="ICsvWriter"/> interface and
    /// extends the <see cref="CsvWriter"/> with templating.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object being written.</typeparam>
    public class CsvWriter<T> : CsvWriter, ICsvWriter
        where T : class
    {
        private readonly PropertyInfo[] _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter{T}"/> class.
        /// </summary>
        public CsvWriter()
        {
            _properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
        }

        protected override string[] GetHeaders(ICsv csv)
        {
            return csv.Headers.Select(h => GetHeaderText(csv as ICsv<T>, h.Title)).ToArray();
        }

        private string GetHeaderText(ICsv<T> csv, string title)
        {
            var property = _properties
                .FirstOrDefault(p => p.Name.Equals(title, System.StringComparison.InvariantCultureIgnoreCase));

            if (property == null)
            {
                return title;
            }

            return csv.HeaderMap.ContainsKey(property)
                ? csv.HeaderMap[property]
                : title;
        }
    }
}
