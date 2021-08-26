using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class CsvHeader : ICsvHeader
    {
        public CsvHeader(int index, string title)
        {
            Index = index;
            Title = title;
        }

        public int Index { get; }

        public string Title { get; }
    }
}
