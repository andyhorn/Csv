using System.Collections.Generic;

namespace Csv.Core.Interfaces
{
    public interface ICsvColumn
    {
        int Index { get; }
        IEnumerable<ICsvCell> Cells { get; }
        ICsvHeader Header { get; }
    }
}
