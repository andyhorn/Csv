using System.Collections.Generic;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class CsvColumn : ICsvColumn
    {
        public CsvColumn(int index, ICsvHeader header)
        {
            Index = index;
            Cells = new List<ICsvCell>();
            Header = header;
        }

        public int Index { get; }

        public IEnumerable<ICsvCell> Cells { get; }

        public ICsvHeader Header { get; internal set; }
    }
}
