using System.Collections.Generic;

namespace Csv.Core.Interfaces
{
    public interface ICsv
    {
        string Filename { get; }
        bool HasHeaders { get; }
        char Separator { get; }
        IEnumerable<ICsvHeader> Headers { get; }
        IEnumerable<ICsvRow> Rows { get; }
        IEnumerable<ICsvColumn> Columns { get; }
        ICsvCell GetCell(int row, int col);
    }
}
