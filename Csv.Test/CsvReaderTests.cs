using System.IO;
using NUnit.Framework;
using System.Linq;
using System;
using Csv.Core.Readers;
using Csv.Core.Interfaces;

namespace Csv.Test
{
    [TestFixture]
    public class CsvReaderTests
    {
        private const string BaseCsvFilename = "BaseTestCsv.csv";
        private ICsvReader _reader;

        [SetUp]
        public void SetUp()
        {
            _reader = new CsvReader();
        }

        [Test]
        public void ReadsCsvFromFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = _reader.FromFile(path);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromStream()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var stream = File.OpenRead(path);
            var csv = _reader.FromStream(stream);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromString()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var data = File.ReadAllText(path);
            var csv = _reader.FromString(data);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReadsCsvFromLines()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var data = File.ReadAllLines(path);
            var csv = _reader.FromLines(data);
            Assert.IsNotNull(csv);
        }

        [Test]
        public void BuildsAllRows()
        {
            const int numRows = 2;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = _reader.FromFile(path);

            Assert.AreEqual(numRows, csv.NumRows);
        }

        [Test]
        public void BuildsAllColumns()
        {
            const int numColumns = 4;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = _reader.FromFile(path);

            Assert.AreEqual(numColumns, csv.Columns.Count());
        }

        [Test]
        public void BuildsAllCells()
        {
            const int numCells = 8;
            var path = Path.Combine(Directory.GetCurrentDirectory(), BaseCsvFilename);
            var csv = _reader.FromFile(path);

            var count = (csv as Core.Models.Csv).Cells.Sum(x => x.Count());

            Assert.AreEqual(numCells, count);
        }

        [Test]
        public void ThrowsExceptionForEmptyFileName()
        {
            var path = string.Empty;

            Assert.Throws<ArgumentNullException>(() => _reader.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForMissingFile()
        {
            var path = "/Some/Garbage/Path";
            Assert.Throws<FileNotFoundException>(() => _reader.FromFile(path));
        }

        [Test]
        public void ThrowsExceptionForNullStream()
        {
            Stream stream = null;
            Assert.Throws<ArgumentException>(() => _reader.FromStream(stream));
        }
    }
}
