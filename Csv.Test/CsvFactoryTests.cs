using Csv.Core;
using Csv.Core.Interfaces;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvFactoryTests
    {
        [Test]
        public void ReturnsNewInstance()
        {
            var csv = CsvFactory.New;

            Assert.IsInstanceOf<ICsv>(csv);
        }

        [Test]
        public void ReturnsNewInstances()
        {
            var csvOne = CsvFactory.New;
            var csvTwo = CsvFactory.New;

            Assert.AreNotSame(csvOne, csvTwo);
        }

        [Test]
        public void ReturnsNewTemplatedInstance()
        {
            var csv = CsvFactory.ForType<TestClass>();

            Assert.IsInstanceOf<ICsv<TestClass>>(csv);
        }

        [Test]
        public void ReturnsNewTemplatedInstances()
        {
            var csvOne = CsvFactory.ForType<TestClass>();
            var csvTwo = CsvFactory.ForType<TestClass>();

            Assert.AreNotSame(csvOne, csvTwo);
        }
    }
}
