using System.Collections.Generic;

namespace Csv.Core.Interfaces
{
    public interface ICsvRow
    {
        int Index { get; }
        IEnumerable<ICsvCell> Cells { get; }
    }
}
