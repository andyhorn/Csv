using System;
using System.Collections.Generic;
using System.Linq;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class Csv : ICsv
    {
        public Csv()
        {
            Headers = new List<ICsvHeader>();
            Rows = new List<ICsvRow>();
            Columns = new List<ICsvColumn>();
        }

        public bool HasHeaders { get; set; }

        public char Separator { get; set; }

        public string Filename { get; set; }

        public IEnumerable<ICsvHeader> Headers { get; set; }

        public IEnumerable<ICsvRow> Rows { get; set; }

        public IEnumerable<ICsvColumn> Columns { get; set; }

        public ICsvCell GetCell(int row, int column)
        {
            if (row >= Rows.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (column >= Columns.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }

            return Rows.ElementAt(row).Cells.ElementAt(column);
        }
    }
}
