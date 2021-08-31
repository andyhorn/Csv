using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Csv.Core.Interfaces;
using Csv.Core.Models;
using Csv.Core.Writers;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvWriterTests
    {
        private ICsvWriter _writer;

        [SetUp]
        public void SetUp()
        {
            _writer = new CsvWriter();
        }

        [Test]
        public void WritesToFile()
        {
            var csv = MakeTestCsv();
            var filePath = Path.GetTempFileName();

            _writer.ToFile(csv, filePath);

            var exists = File.Exists(filePath);

            if (exists)
            {
                File.Delete(filePath);
            }

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task WritesToFileAsync()
        {
            var csv = MakeTestCsv();
            var filePath = Path.GetTempFileName();

            await _writer.ToFileAsync(csv, filePath);

            var exists = File.Exists(filePath);

            if (exists)
            {
                File.Delete(filePath);
            }

            Assert.IsTrue(exists);
        }

        [Test]
        public void WritesToStream()
        {
            var csv = MakeTestCsv();
            var filePath = Path.GetTempFileName();
            var stream = File.OpenWrite(filePath);

            _writer.ToStream(csv, stream);

            var exists = File.Exists(filePath);

            if (exists)
            {
                File.Delete(filePath);
            }

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task WritesToStreamAsync()
        {
            var csv = MakeTestCsv();
            var filePath = Path.GetTempFileName();
            var stream = File.OpenWrite(filePath);

            await _writer.ToStreamAsync(csv, stream);

            var exists = File.Exists(filePath);

            if (exists)
            {
                File.Delete(filePath);
            }

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task FileContainsCsvData()
        {
            var csv = MakeTestCsv();
            var filePath = Path.GetTempFileName();
            var headerContent = string.Join(csv.Separator, csv.Headers.Select(x => x.Title));

            await _writer.ToFileAsync(csv, filePath);

            var exists = File.Exists(filePath);

            if (!exists)
            {
                Assert.Fail("File was not written to disk.");
            }

            var fileData = File.ReadAllLines(filePath);

            File.Delete(filePath);

            Assert.AreEqual(headerContent, fileData[0]);

            for (var i = 1; i < fileData.Count(); i++)
            {
                Assert.AreEqual(string.Join(csv.Separator, csv.Cells[i - 1].Select(x => x.Value)), fileData[i]);
            }
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
