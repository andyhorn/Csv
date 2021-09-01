using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Csv.Core.Interfaces;
using Csv.Core.Writers;
using Csv.Test.Utilities;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvWriterTemplatedTests
    {
        private readonly CsvFactory _factory;
        private CsvWriter<TestClass> _writer;
        private ICsv<TestClass> _csv;
        private string _tempFilePath;

        public CsvWriterTemplatedTests()
        {
            _factory = new CsvFactory();
        }

        [SetUp]
        public void SetUp()
        {
            _writer = new CsvWriter<TestClass>();
            _csv = _factory.Make<TestClass>();
            _tempFilePath = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [Test]
        public void WritesToFile()
        {
            _writer.ToFile(_csv, _tempFilePath);

            var exists = File.Exists(_tempFilePath);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task WritesToFileAsync()
        {
            await _writer.ToFileAsync(_csv, _tempFilePath);

            var exists = File.Exists(_tempFilePath);

            Assert.IsTrue(exists);
        }

        [Test]
        public void WritesToStream()
        {
            var stream = File.OpenWrite(_tempFilePath);

            _writer.ToStream(_csv, stream);

            var exists = File.Exists(_tempFilePath);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task WritesToStreamAsync()
        {
            var stream = File.OpenWrite(_tempFilePath);

            await _writer.ToStreamAsync(_csv, stream);

            var exists = File.Exists(_tempFilePath);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task FileContainsCsvData()
        {
            var headerContent = string.Join(_csv.Separator, _csv.Headers.Select(x => x.Title));

            await _writer.ToFileAsync(_csv, _tempFilePath);

            var exists = File.Exists(_tempFilePath);

            if (!exists)
            {
                Assert.Fail("File was not written to disk.");
            }

            var fileData = File.ReadAllLines(_tempFilePath);
            Assert.AreEqual(headerContent, fileData[0]);

            for (var i = 1; i < fileData.Count(); i++)
            {
                Assert.AreEqual(string.Join(_csv.Separator, _csv.Cells[i - 1].Select(c => c.Value)), fileData[i]);
            }
        }
    }
}
