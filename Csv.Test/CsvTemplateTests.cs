using System;
using System.Collections.Generic;
using System.IO;
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
        public void CannotAddPropertyDirectlyToList()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));
            _uut.Ignores.Add(property);
            Assert.IsFalse(_uut.Ignores.Contains(property));
        }

        [Test]
        public void Add_IgnoresProperties()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));

            _uut.IgnoreProperty(property);
            _uut.Add(_testClass);

            Assert.IsFalse(_uut.Headers.Any(h => h.Title.Equals(property.Name)));
        }

        [Test]
        public void CannotAddMapDirectly()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));

            _uut.HeaderMap.Add(property, "Test");

            Assert.IsFalse(_uut.HeaderMap.ContainsKey(property));
        }

        [Test]
        public void HeaderMapAddedToDictionary()
        {
            const string title = "Test";
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));

            _uut.AddHeaderMap(property, title);

            Assert.IsTrue(_uut.HeaderMap.ContainsKey(property));
            Assert.AreEqual(title, _uut.HeaderMap[property]);
        }

        [Test]
        public void HeaderMapChangesHeaders()
        {
            const int index = 1;
            const string newTitle = "Test";
            var header = _uut.Headers[index];
            var title = header.Title;
            var property = typeof(TestClass).GetProperty(title);

            _uut.AddHeaderMap(property, newTitle);

            Assert.AreEqual(newTitle, _uut.Headers[index].Title);
        }

        [Test]
        public void HeaderMapCtorChangesHeaders()
        {
            const string title = "TITLE";
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));
            var map = new Dictionary<PropertyInfo, string>
            {
                { property, title },
            };

            _uut = new Csv<TestClass>(headerMap: map);

            Assert.IsTrue(_uut.HeaderMap.ContainsKey(property));
            Assert.AreEqual(title, _uut.HeaderMap[property]);
            Assert.IsFalse(_uut.Headers.Any(h => h.Title.Equals(nameof(TestClass.Text))));
        }

        [Test]
        public void AddNullPropertyToHeaderMapThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _uut.AddHeaderMap(null, "Test"));
        }

        [Test]
        public void AddEmptyStringToHeaderMapThrowsException()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Text));
            Assert.Throws<ArgumentNullException>(() => _uut.AddHeaderMap(property, string.Empty));
        }

        [Test]
        public void CannotAddPropertyFromDifferentObject()
        {
            var property = typeof(FileInfo).GetProperty(nameof(FileInfo.CreationTime));
            Assert.Throws<ArgumentException>(() => _uut.AddHeaderMap(property, "Test"));
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

            _uut = new Csv<TestClass>(headerMap: map);

            _uut.Add(_testClass);

            Assert.IsFalse(_uut.Headers.Any(h => h.Title.Equals(nameof(TestClass.Text))));
            Assert.IsTrue(_uut.Headers.Any(h => h.Title.Equals(customHeader)));
        }
    }
}
