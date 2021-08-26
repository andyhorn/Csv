using System.IO;
using NUnit.Framework;
using System.Linq;
using System;
using Csv.Core;

namespace Csv.Test
{
    [TestFixture]
    public class CsvReaderTests
    {
        private const string BaseCsvFilename = "BaseTestCsv.csv";

        [Test]
        public void ReadsCsvFromFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = CsvReader.FromFile(path);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromStream()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var stream = File.OpenRead(path);
            var csv = CsvReader.FromStream(stream);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromString()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var data = File.ReadAllText(path);
            var csv = CsvReader.FromString(data);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromLines()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var data = File.ReadAllLines(path);
            var csv = CsvReader.FromLines(data);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void BuildsAllRows()
        {
            const int numRows = 2;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = CsvReader.FromFile(path);

            Assert.AreEqual(numRows, csv.Rows.Count());
        }

        [Test]
        public void BuildsAllColumns()
        {
            const int numColumns = 4;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = CsvReader.FromFile(path);

            Assert.AreEqual(numColumns, csv.Columns.Count());
        }

        [Test]
        public void BuildsAllCells()
        {
            const int numCells = 8;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = CsvReader.FromFile(path);

            var count = csv.Rows.Sum(r => r.Cells.Count());

            Assert.AreEqual(numCells, count);
        }

        [Test]
        public void ThrowsExceptionForEmptyFileName()
        {
            var path = string.Empty;

            Assert.Throws<ArgumentNullException>(() => CsvReader.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForMissingFile()
        {
            var path = "/Some/Garbage/Path";
            Assert.Throws<FileNotFoundException>(() => CsvReader.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForNullStream()
        {
            Stream stream = null;
            Assert.Throws<ArgumentException>(() => CsvReader.FromStream(stream));
        }
    }
}
