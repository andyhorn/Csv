using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csv.Core.Interfaces;
using Csv.Core.Models;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvTemplateTests
    {
        private readonly PropertyInfo[] _properties;
        private ICsv<TestClass> _uut;
        private TestClass _testClass;

        public CsvTemplateTests()
        {
            _properties = typeof(TestClass)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
        }

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

        [Test]
        public void Add_AddsOneRowToCsv()
        {
            var originalNumRows = _uut.NumRows;
            _uut.Add(_testClass);

            Assert.AreEqual(originalNumRows + 1, _uut.NumRows);
        }

        [Test]
        public void Add_AddsItemValuesToCells()
        {
            _uut.Add(_testClass);

            foreach (var property in _properties)
            {
                var originalValue = property.GetValue(_testClass);
                var csvIndex = _uut.Headers.FirstOrDefault(h => h.Title.Equals(property.Name)).Index;

                Assert.AreEqual(originalValue, _uut.Cells[0][csvIndex].Value);
            }
        }

        [Test]
        public void Add_IgnoresProperties()
        {
            _uut.Ignores.Add(typeof(TestClass).GetProperty(nameof(TestClass.Float)));

            _uut.Add(_testClass);

            Assert.IsFalse(_uut.Headers.Any(h => h.Title.Equals(nameof(TestClass.Float))));
        }

        [Test]
        public void Add_NullIgnoreList()
        {
            _uut.Ignores = null;

            Assert.DoesNotThrow(() => _uut.Add(_testClass));
        }

        [Test]
        public void Add_NullItemThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _uut.Add(null));
        }

        [Test]
        public void AddRange_AddsAllItems()
        {
            var items = new TestClass[]
            {
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Test",
                },
                new TestClass
                {
                    Boolean = false,
                    Float = 4.56f,
                    Number = 456,
                    Text = "Test",
                },
                new TestClass
                {
                    Boolean = true,
                    Float = 7.89f,
                    Number = 789,
                    Text = "Test",
                },
            };

            _uut.AddRange(items);

            Assert.AreEqual(3, _uut.NumRows);
        }

        [Test]
        public void AddRange_NullItemIsSkipped()
        {
            var items = new TestClass[]
            {
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Test",
                },
                null,
                new TestClass
                {
                    Boolean = false,
                    Float = 4.56f,
                    Number = 456,
                    Text = "Test",
                },
            };

            _uut.AddRange(items);

            Assert.AreEqual(2, _uut.NumRows);
        }

        [Test]
        public void Get_RetrievesItem()
        {
            _uut.Add(_testClass);

            var item = _uut.Get(0);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf<TestClass>(item);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void Get_InvalidIndexThrowsException(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _uut.Get(index));
        }

        [Test]
        public void Get_ReturnsCollectionOfItems()
        {
            var items = _uut.Get();

            Assert.IsNotNull(items);
            Assert.IsInstanceOf<ICollection<TestClass>>(items);
        }

        [Test]
        public void Get_ReturnsAllItems()
        {
            const int numItems = 3;
            for (var i = 0; i < numItems; i++)
            {
                _uut.Add(_testClass);
            }

            var items = _uut.Get();

            Assert.AreEqual(numItems, items.Count());
        }

        [Test]
        public void Remove_RemovesOneRow()
        {
            var items = new TestClass[]
            {
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Keep",
                },
                new TestClass
                {
                    Boolean = false,
                    Float = 4.56f,
                    Number = 456,
                    Text = "Remove",
                },
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Keep",
                },
            };

            _uut.AddRange(items);

            _uut.Remove(0);

            Assert.AreEqual(items.Count() - 1, _uut.NumRows);
        }

        [Test]
        public void Remove_RemovesCorrectItem()
        {
            var items = new TestClass[]
            {
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Keep",
                },
                new TestClass
                {
                    Boolean = false,
                    Float = 4.56f,
                    Number = 456,
                    Text = "Remove",
                },
                new TestClass
                {
                    Boolean = true,
                    Float = 1.23f,
                    Number = 123,
                    Text = "Keep",
                },
            };

            _uut.AddRange(items);

            _uut.Remove(1);

            var textIndex = _uut.Headers
                .FirstOrDefault(h => h.Title.Equals(nameof(TestClass.Text)))
                .Index;

            foreach (var row in _uut.Rows)
            {
                Assert.AreNotEqual("Remove", row.Cells[textIndex].Value);
            }
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void Remove_InvalidIndexThrowsException(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _uut.Remove(index));
        }

        [Test]
        public void HeaderMapChangesHeaderText()
        {
            const string customHeader = "Custom Header Text";
            var map = new Dictionary<PropertyInfo, string>
            {
                { typeof(TestClass).GetProperty(nameof(TestClass.Text)), customHeader },
            };

            _uut.HeaderMap = map;

            _uut.Add(_testClass);

            Assert.IsFalse(_uut.Headers.Any(h => h.Title.Equals(nameof(TestClass.Text))));
            Assert.IsTrue(_uut.Headers.Any(h => h.Title.Equals(customHeader)));
        }
    }
}
