using System.IO;
using NUnit.Framework;
using System.Linq;
using System;

namespace Csv.Test
{
    [TestFixture]
    public class CsvTests
    {
        private const string BaseCsvFilename = "BaseTestCsv.csv";

        [Test]
        public void ReadsCsvFromFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = Csv.Core.Csv.FromFile(path);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromStream()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var stream = File.OpenRead(path);
            var csv = Csv.Core.Csv.FromStream(stream);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void BuildsAllRows()
        {
            const int numRows = 2;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = Csv.Core.Csv.FromFile(path);

            Assert.AreEqual(numRows, csv.Rows.Count());
        }

        [Test]
        public void BuildsAllColumns()
        {
            const int numColumns = 4;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = Csv.Core.Csv.FromFile(path);

            Assert.AreEqual(numColumns, csv.Columns.Count());
        }

        [Test]
        public void BuildsAllCells()
        {
            const int numCells = 8;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = Core.Csv.FromFile(path);

            var count = csv.Rows.Sum(r => r.Cells.Count());

            Assert.AreEqual(numCells, count);
        }

        [Test]
        public void ThrowsExceptionForEmptyFileName()
        {
            var path = string.Empty;

            Assert.Throws<ArgumentNullException>(() => Csv.Core.Csv.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForMissingFile()
        {
            var path = "/Some/Garbage/Path";
            Assert.Throws<FileNotFoundException>(() => Core.Csv.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForNullStream()
        {
            Stream stream = null;
            Assert.Throws<ArgumentException>(() => Core.Csv.FromStream(stream));
        }
    }
}
