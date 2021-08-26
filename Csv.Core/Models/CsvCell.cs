using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class CsvCell : ICsvCell
    {
        public CsvCell(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public ICsvRow Row { get; internal set; }

        public ICsvColumn Column { get; internal set; }
    }
}
