using System.IO;
using System.Linq;
using Csv.Core.Interfaces;
using Csv.Core.Readers;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvReaderTemplatedTests
    {
        private const string CsvFilename = "TestClassCsv.csv";
        private CsvReader<TestClass> _reader;
        private string _csvFilepath;

        [SetUp]
        public void SetUp()
        {
            _reader = new CsvReader<TestClass>();
            _csvFilepath = Path.Combine(Directory.GetCurrentDirectory(), CsvFilename);
        }

        [Test]
        public void ReadsCsvFromFile()
        {
            var csv = _reader.FromFile(_csvFilepath);

            Assert.IsNotNull(csv);
            Assert.IsInstanceOf<ICsv<TestClass>>(csv);
        }

        [Test]
        public void ReadsCsvFromStream()
        {
            var stream = File.OpenRead(_csvFilepath);
            var csv = _reader.FromStream(stream);

            Assert.IsNotNull(csv);
            Assert.IsInstanceOf<ICsv<TestClass>>(csv);
        }

        [Test]
        public void ReadsCsvFromString()
        {
            var data = File.ReadAllText(_csvFilepath);
            var csv = _reader.FromString(data);

            Assert.IsNotNull(csv);
            Assert.IsInstanceOf<ICsv<TestClass>>(csv);
        }

        [Test]
        public void ReadsCsvFromLines()
        {
            var data = File.ReadAllLines(_csvFilepath);
            var csv = _reader.FromLines(data);

            Assert.IsNotNull(csv);
            Assert.IsInstanceOf<ICsv<TestClass>>(csv);
        }

        [Test]
        public void BuildsAllRows()
        {
            const int numRows = 3;
            var csv = _reader.FromFile(_csvFilepath);

            Assert.AreEqual(numRows, csv.NumRows);
        }

        [Test]
        public void BuildsAllColumns()
        {
            const int numColumns = 4;
            var csv = _reader.FromFile(_csvFilepath);

            Assert.AreEqual(numColumns, csv.NumColumns);
        }

        [Test]
        public void BuildsAllCells()
        {
            const int numCells = 12;
            var csv = _reader.FromFile(_csvFilepath);
            var count = csv.Cells.Sum(x => x.Count());

            Assert.AreEqual(numCells, count);
        }
    }
}
