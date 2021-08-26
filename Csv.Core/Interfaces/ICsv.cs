using System.Collections.Generic;

namespace Csv.Core.Interfaces
{
    public interface ICsv
    {
        string Filename { get; set; }
        IEnumerable<ICsvHeader> Headers { get; }
        IEnumerable<ICsvRow> Rows { get; }
        IEnumerable<ICsvColumn> Columns { get; }
    }
}
