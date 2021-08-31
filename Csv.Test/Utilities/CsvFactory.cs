using Csv.Core.Interfaces;
using Csv.Core.Models;

namespace Csv.Test.Utilities
{
    public class CsvFactory
    {
        public ICsv Make()
        {
            var csv = new Core.Models.Csv
            {
                Headers = MakeHeaders(3),
            };

            csv.Cells = MakeCells(csv, 3, 3);

            return csv;
        }

        public ICsv<T> Make<T>()
        {
            var csv = new Csv<T>();

            csv.Cells = MakeCells(csv, 3, 3);

            return csv;
        }

        private ICsvHeader[] MakeHeaders(int numHeaders)
        {
            var headers = new ICsvHeader[numHeaders];

            for (var i = 0; i < numHeaders; i++)
            {
                var header = new CsvHeader(i, $"Header {i + 1}");
                headers[i] = header;
            }

            return headers;
        }

        private ICsvCell[][] MakeCells(ICsv csv, int numRows, int numColumns)
        {
            var cells = new ICsvCell[numRows][];

            for (var row = 0; row < numRows; row++)
            {
                cells[row] = new ICsvCell[numColumns];

                for (var column = 0; column < numColumns; column++)
                {
                    var value = $"Cell {numColumns * numRows + numColumns + 1}";
                    cells[row][column] = new CsvCell(csv, row, column, value);
                }
            }

            return cells;
        }
    }
}
