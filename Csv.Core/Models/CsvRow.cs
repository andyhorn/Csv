using System.Collections.Generic;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class CsvRow : ICsvRow
    {
        public CsvRow(int index)
        {
            Index = index;
            Cells = new List<ICsvCell>();
        }

        public int Index { get; }

        public IEnumerable<ICsvCell> Cells { get; }
    }
}
