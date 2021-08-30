using System;
using System.Collections.Generic;
using System.Linq;
using Csv.Core.Interfaces;
using Csv.Core.Models;
using NUnit.Framework;

namespace Csv.Test
{
    [TestFixture]
    public class CsvCollectionTests
    {
        private class TestClass
        {
            public string Text { get; set; }
            public int Number { get; set; }
            public bool Boolean { get; set; }
        }

        private TestClass _testClassInstance;
        private ICsv _csvInstance;
        private CsvCollection<TestClass> _collection;

        [SetUp]
        public void SetUp()
        {
            _testClassInstance = new TestClass
            {
                Text = "Test",
                Number = 999,
                Boolean = true,
            };

            _csvInstance = new Core.Models.Csv();
            _collection = new CsvCollection<TestClass>(_csvInstance);
        }

        [Test]
        public void DefaultCtorTest()
        {
            _ = new CsvCollection<TestClass>();
            Assert.Pass();
        }

        [Test]
        public void ParameterizedCtorTest()
        {
            _ = new CsvCollection<TestClass>(_csvInstance);

            Assert.Pass();
        }

        [Test]
        public void AddItemAddsToCsv()
        {
            _collection.Add(_testClassInstance);

            Assert.IsNotEmpty(_csvInstance.Cells);
        }

        [Test]
        public void AddItemCreatesHeaders()
        {
            _collection.Add(_testClassInstance);
            Assert.AreEqual(3, _csvInstance.Headers.Count());
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void Get_InvalidIndexThrowsException(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _collection.Get(index));
        }

        [Test]
        public void Get_ReturnsInstanceOfItem()
        {
            _collection.Add(_testClassInstance);

            var item = _collection.Get(0);

            Assert.IsInstanceOf<TestClass>(item);
        }

        [Test]
        public void Get_ReturnsItemWithValues()
        {
            _collection.Add(_testClassInstance);

            var item = _collection.Get(0);

            Assert.AreEqual(_testClassInstance.Text, item.Text);
            Assert.AreEqual(_testClassInstance.Number, item.Number);
            Assert.AreEqual(_testClassInstance.Boolean, item.Boolean);
        }

        [Test]
        public void GetReturnsCollectionOfItems()
        {
            _collection.Add(_testClassInstance);

            var collection = _collection.Get();

            Assert.IsInstanceOf<ICollection<TestClass>>(collection);
        }

        [Test]
        public void GetReturnsProperNumberOfItems()
        {
            const int numItems = 3;

            for (var i = 0; i < numItems; i++)
            {
                _collection.Add(_testClassInstance);
            }

            var collection = _collection.Get();

            Assert.AreEqual(numItems, collection.Count());
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void RemoveInvalidIndexThrowsException(int index)
        {
            _collection.Add(_testClassInstance);
            Assert.Throws<ArgumentOutOfRangeException>(() => _collection.Remove(index));
        }

        [Test]
        public void RemoveRemovesRow()
        {
            _collection.Add(_testClassInstance);
            _collection.Remove(0);

            Assert.IsEmpty(_csvInstance.Cells);
        }

        [Test]
        public void RemoveRemovesCorrectRow()
        {
            var testOne = new TestClass
            {
                Text = "Test Class One",
                Number = 100,
                Boolean = true,
            };
            var testTwo = new TestClass
            {
                Text = "Test Class Two",
                Number = 200,
                Boolean = false,
            };

            _collection.Add(testOne);
            _collection.Add(testTwo);

            _collection.Remove(0);

            var itemText = (string)_csvInstance.Cells.First().ElementAt(0).Value;
            var itemNumber = (int)_csvInstance.Cells.First().ElementAt(1).Value;
            var itemBoolean = (bool)_csvInstance.Cells.First().ElementAt(2).Value;

            Assert.AreEqual(testTwo.Text, itemText);
            Assert.AreEqual(testTwo.Number, itemNumber);
            Assert.AreEqual(testTwo.Boolean, itemBoolean);
        }

        [Test]
        public void HeaderMapChangesHeaderNames()
        {
            const string textHeader = "Text Header";
            const string numberHeader = "Number Header";
            const string booleanHeader = "Boolean Header";

            _collection = new CsvCollection<TestClass>(_csvInstance)
            {
                Options = new CsvCollectionOptions
                {
                    HeaderMap = new Dictionary<string, string>
                    {
                        {nameof(TestClass.Text), textHeader},
                        {nameof(TestClass.Number), numberHeader},
                        {nameof(TestClass.Boolean), booleanHeader},
                    },
                },
            };

            _collection.Add(_testClassInstance);

            Assert.IsTrue(_csvInstance.Headers.Any(h => h.Title.Equals(textHeader)));
            Assert.IsTrue(_csvInstance.Headers.Any(h => h.Title.Equals(numberHeader)));
            Assert.IsTrue(_csvInstance.Headers.Any(h => h.Title.Equals(booleanHeader)));
        }

        [Test]
        public void MappedHeadersRestoreValues()
        {
            const string textHeader = "Text Header";
            const string numberHeader = "Number Header";
            const string booleanHeader = "Boolean Header";

            _collection = new CsvCollection<TestClass>(_csvInstance)
            {
                Options = new CsvCollectionOptions
                {
                    HeaderMap = new Dictionary<string, string>
                    {
                        {nameof(TestClass.Text), textHeader},
                        {nameof(TestClass.Number), numberHeader},
                        {nameof(TestClass.Boolean), booleanHeader},
                    },
                },
            };

            _collection.Add(_testClassInstance);

            var item = _collection.Get(0);

            Assert.AreEqual(_testClassInstance.Text, item.Text);
            Assert.AreEqual(_testClassInstance.Number, item.Number);
            Assert.AreEqual(_testClassInstance.Boolean, item.Boolean);
        }

        [Test]
        public void InvalidMappedHeadersRevertToPropertyNames()
        {
            const string invalidTextHeader = "Text Header";
            const string invalidNumberHeader = "Number Header";
            const string invalidBooleanHeader = "Boolean Header";

            _collection = new CsvCollection<TestClass>(_csvInstance)
            {
                Options = new CsvCollectionOptions
                {
                    HeaderMap = new Dictionary<string, string>
                    {
                        {"TestClassText", invalidTextHeader},
                        {"TestClassNumber", invalidNumberHeader},
                        {"TestClassBoolean", invalidBooleanHeader},
                    },
                },
            };

            _collection.Add(_testClassInstance);

            var headerNames = _csvInstance.Headers.Select(h => h.Title).ToList();

            Assert.AreEqual(3, headerNames.Count());
            Assert.Contains(nameof(TestClass.Text), headerNames);
            Assert.Contains(nameof(TestClass.Number), headerNames);
            Assert.Contains(nameof(TestClass.Boolean), headerNames);
        }

        [Test]
        public void PrettyPrintsContents()
        {
            _collection.AddRange(new List<TestClass>
            {
                _testClassInstance,
                new TestClass
                {
                    Text = "Test Class Two",
                    Number = 10000,
                    Boolean = false,
                },
                new TestClass
                {
                    Text = "Test Class Three",
                    Number = 123,
                    Boolean = false,
                },
            });

            var text = _collection.Print();

            Assert.IsNotEmpty(text);
        }
    }
}
