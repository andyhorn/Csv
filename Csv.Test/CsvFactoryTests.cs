using Csv.Core;
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

            Assert.IsNotNull(csv);
        }

        [Test]
        public void ReturnsNewInstances()
        {
            var csvOne = CsvFactory.New;
            var csvTwo = CsvFactory.New;

            Assert.AreNotSame(csvOne, csvTwo);
        }
    }
}
