using System;
using Csv.Core.Interfaces;
using Csv.Core.Models;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvTemplateTests
    {
        private ICsv<TestClass> _uut;
        private TestClass _testClass;

        [SetUp]
        public void SetUp()
        {
            _uut = new Csv<TestClass>();
            _testClass = new TestClass
            {
                Text = "Test",
                Boolean = true,
                Float = 1.23f,
                Number = 123,
            };
        }
    }
}
