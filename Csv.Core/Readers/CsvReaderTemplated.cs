using System.IO;
using Csv.Core.Interfaces;
using Csv.Core.Models;

namespace Csv.Core.Readers
{
    /// <summary>
    /// Implements the <see cref="ICsvReader"/> interface and
    /// extends the <see cref="CsvReader"/> class to produce
    /// templated <see cref="CsvReader{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object
    /// to be produced from the source.</typeparam>
    public class CsvReader<T> : CsvReader, ICsvReader
        where T : class
    {
        protected override ICsv GetCsv()
        {
            return new Csv<T>
            {
                Separator = Separator,
            };
        }

        protected override void SetHeaders(ICsv csv, string[] lines)
        {
            return;
        }

        protected override void SetHeaders(ICsv csv, StreamReader reader)
        {
            return;
        }
    }
}
