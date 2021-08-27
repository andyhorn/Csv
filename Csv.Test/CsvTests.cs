using System;
using Csv.Core.Interfaces;
using Csv.Core.Models;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvTests
    {
        [Test]
        public void GetCell_ReturnsCell()
        {
            var csv = MakeTestCsv();

            var cell = csv.GetCell(0, 0);

            Assert.IsNotNull(cell);
            Assert.AreEqual("CellOne", cell.Value);
        }

        [Test]
        public void SetCell_SetsCellContent()
        {
            var csv = MakeTestCsv();
            var newContent = "NewContent";

            csv.SetCell(0, 0, newContent);

            Assert.AreEqual(newContent, (csv as Core.Models.Csv).Cells[0][0].Value);
        }

        [Test]
        public void SetCell_CreatesNewColumns()
        {
            const string newValue = "NewValue";
            var csv = MakeTestCsv();
            var newColumnIndex = csv.NumColumns;

            csv.SetCell(0, newColumnIndex, newValue);

            Assert.AreEqual(newColumnIndex + 1, csv.NumColumns);
            Assert.AreEqual(newValue, csv.GetCell(0, newColumnIndex).Value);
        }

        [Test]
        public void SetCell_CreatesNewRows()
        {
            const string newValue = "NewValue";
            var csv = MakeTestCsv();
            var newRowIndex = csv.NumRows;

            csv.SetCell(newRowIndex, 0, newValue);

            Assert.AreEqual(newRowIndex + 1, csv.NumRows);
            Assert.AreEqual(newValue, csv.GetCell(newRowIndex, 0).Value);
        }

        [Test]
        public void SetCell_NewRows_EmptyCellsAreNotNull()
        {
            var csv = MakeTestCsv();
            var originalNumRows = csv.NumRows;
            var newNumRows = csv.NumRows + 100;
            var newCellRowIndex = newNumRows - 1;

            csv.SetCell(newCellRowIndex, 0, "NewCell");

            for (var i = originalNumRows; i < newCellRowIndex; i++)
            {
                Assert.IsNotNull(csv.GetCell(i, 0));
            }
        }

        [Test]
        public void SetCell_NewColumns_EmptyCellsAreNotNull()
        {
            var csv = MakeTestCsv();
            var originalNumColumns = csv.NumColumns;
            var newNumColumns = originalNumColumns + 100;
            var newCellColumnIndex = newNumColumns - 1;

            csv.SetCell(0, newCellColumnIndex, "NewCell");

            for (var i = originalNumColumns; i < newNumColumns; i++)
            {
                Assert.IsNotNull(csv.GetCell(0, i));
            }
        }

        [Test]
        public void Rows_GetsAppropriateRow()
        {
            const int rowIndex = 1;
            var csv = MakeTestCsv();
            var row = csv.Rows[rowIndex];

            Assert.AreEqual(rowIndex, row.Index);
        }

        [Test]
        public void Columns_GetsAppropriateColumn()
        {
            const int columnIndex = 1;
            var csv = MakeTestCsv();
            var column = csv.Columns[columnIndex];

            Assert.AreEqual(columnIndex, column.Index);
        }

        [Test]
        public void ColumnContainsSameCellReference()
        {
            var csv = MakeTestCsv();
            var column = csv.Columns[0];

            Assert.AreSame(csv.Cells[0][0], column.Cells[0]);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void GetCell_ThrowsExceptionForInvalidRowIndex(int index)
        {
            var csv = MakeTestCsv();

            Assert.Throws<ArgumentOutOfRangeException>(() => csv.GetCell(index, 0));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void GetCell_ThrowsExceptionForInvalidColumnIndex(int index)
        {
            var csv = MakeTestCsv();

            Assert.Throws<ArgumentOutOfRangeException>(() => csv.GetCell(0, index));
        }

        [Test]
        [TestCase(-1)]
        public void SetCell_ThrowsExceptionForInvalidRowIndex(int index)
        {
            var csv = MakeTestCsv();

            Assert.Throws<ArgumentOutOfRangeException>(() => csv.SetCell(index, 0, "Test"));
        }

        [Test]
        [TestCase(-1)]
        public void SetCell_ThrowsExceptionForInvalidColumnIndex(int index)
        {
            var csv = MakeTestCsv();

            Assert.Throws<ArgumentOutOfRangeException>(() => csv.SetCell(0, index, "Test"));
        }

        private static ICsv MakeTestCsv()
        {
            var csv = new Core.Models.Csv
            {
                Headers = new ICsvHeader[]
                {
                    new CsvHeader(0, "HeaderOne"),
                    new CsvHeader(1, "HeaderTwo"),
                    new CsvHeader(2, "HeaderThree"),
                },
            };

            csv.Cells = new ICsvCell[][]
            {
                new ICsvCell[]
                {
                    new CsvCell(csv, 0, 0, "CellOne"),
                    new CsvCell(csv, 0, 1, "CellTwo"),
                    new CsvCell(csv, 0, 2, "CellThree"),
                },
                new ICsvCell[]
                {
                    new CsvCell(csv, 1, 0, "CellFour"),
                    new CsvCell(csv, 1, 1, "CellFive"),
                    new CsvCell(csv, 1, 2, "CellSix"),
                },
            };

            return csv;
        }
    }
}
