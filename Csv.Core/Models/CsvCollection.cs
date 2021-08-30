using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    /// <summary>
    /// Manages a collection of items in CSV format.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> held in the collection.</typeparam>
    public class CsvCollection<T>
    {
        private readonly ICsv _csv;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvCollection{T}"/> class.
        /// </summary>
        /// <param name="csv">An instance of the <see cref="ICsv"/> interface
        /// to be used by the collection.</param>
        public CsvCollection(ICsv csv)
        {
            _csv = csv;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvCollection{T}"/> class.
        /// </summary>
        public CsvCollection()
        {
            _csv = CsvFactory.New;
        }

        /// <summary>
        /// Gets or sets the options for this collection.
        /// </summary>
        public CsvCollectionOptions Options { get; set; }

        /// <summary>
        /// Adds a new item to the CSV document.
        /// </summary>
        /// <param name="item">The item of type <typeparamref name="T"/> to be
        /// added to the internal CSV document..</param>
        public void Add(T item)
        {
            var rowIndex = _csv.NumRows;
            var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.PropertyType.IsPrimitive && !property.PropertyType.Equals(typeof(string)))
                {
                    continue;
                }

                object value = property.GetValue(item);
                var index = GetColumnIndex(property);
                _csv.SetCell(rowIndex, index, value);
            }
        }

        /// <summary>
        /// Adds a collection of items of type <typeparamref name="T"/> to the
        /// internal CSV document.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(ICollection<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Gets all items of type <typeparamref name="T"/> from the CSV
        /// document and casts to C# objects.
        /// </summary>
        /// <returns>An <see cref="ICollection{T}"/> of type <typeparamref name="T"/>.</returns>
        public ICollection<T> Get()
        {
            var items = new List<T>();

            for (var row = 0; row < _csv.NumRows; row++)
            {
                var item = Get(row);

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Gets a single item of type <typeparamref name="T"/> from the CSV
        /// document at the specified index.
        /// </summary>
        /// <param name="index">The index at which to retrieve the item.</param>
        /// <returns>An item of type <typeparamref name="T"/>.</returns>
        public T Get(int index)
        {
            if (index < 0 || index >= _csv.NumRows)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var item = (T)Activator.CreateInstance(typeof(T));

            foreach (var header in _csv.Headers)
            {
                var property = GetProperty(header.Title);
                object value = _csv.Cells[index][header.Index].Value;
                property.SetValue(item, value);
            }

            return item;
        }

        /// <summary>
        /// Removes an existing item from the internal CSV document at the
        /// specified index.
        /// </summary>
        /// <param name="index">The index at which to remove the item.</param>
        public void Remove(int index)
        {
            if (index < 0 || index >= _csv.NumRows)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var cells = _csv.Cells
                .Select((row, i) => i != index ? row : null)
                .Where(r => r != null)
                .ToArray();

            (_csv as Csv).Cells = cells;
        }

        /// <summary>
        /// Returns a "pretty-print" string containing the headers and all data
        /// in table format.
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            var columnWidths = GetColumnWidths();
            var builder = new StringBuilder();

            if (_csv.HasHeaders)
            {
                builder.AppendLine(BuildRow(_csv.Headers.Select(x => x.Title).ToArray(), columnWidths.ToArray(), true));
            }

            var sum = columnWidths.Select(x => x + 4).Sum() + columnWidths.Count();
            builder.AppendLine(new string('-', sum));

            foreach (var row in _csv.Rows)
            {
                var items = row.Cells.Select(c => c.Value.ToString()).ToArray();
                builder.AppendLine(BuildRow(items, columnWidths.ToArray()));
            }

            return builder.ToString();
        }

        private string BuildRow(object[] values, int[] widths, bool center = false)
        {
            var items = new List<string>();

            for (var i = 0; i < values.Length; i++)
            {
                var text = values[i].ToString();
                var totalWidth = widths[i] + 4;
                var emptySpace = totalWidth - text.Count();

                if (!center)
                {
                    items.Add(text.PadLeft(text.Count() + 2).PadRight(widths[i] + 4));
                }
                else
                {
                    var sideSpace = emptySpace / 2;
                    items.Add(text.PadLeft(text.Count() + sideSpace).PadRight(totalWidth));
                }
            }

            return string.Join('|', items);
        }

        private List<int> GetColumnWidths()
        {
            var widths = new List<int>();

            for (var i = 0; i < _csv.NumColumns; i++)
            {
                var header = _csv.HasHeaders ? _csv.Headers.ElementAt(i).Title.Count() : 0;
                var column = _csv.Cells
                    .Select(x => x.ElementAt(i).Value.ToString().Count())
                    .Max();

                widths.Add(Math.Max(header, column));
            }

            return widths;
        }

        private int GetColumnIndex(PropertyInfo property)
        {
            var name = property.Name;

            if (Options != null && Options.HeaderMap != null && Options.HeaderMap.ContainsKey(name))
            {
                name = Options.HeaderMap[name];
                var header = _csv.Headers.FirstOrDefault(h => h.Title.Equals(name));

                if (header != null)
                {
                    return header.Index;
                }
            }

            if (_csv.HasHeaders)
            {
                var header = _csv.Headers.FirstOrDefault(h => h.Title.Equals(name, StringComparison.CurrentCultureIgnoreCase));

                if (header != null)
                {
                    return header.Index;
                }
            }

            var newHeader = new CsvHeader(_csv.NumColumns, name);
            _csv.Headers = _csv.Headers.Append(newHeader).ToArray();

            return newHeader.Index;
        }

        private PropertyInfo GetProperty(string headerTitle)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (Options != null && Options.HeaderMap != null && Options.HeaderMap.ContainsValue(headerTitle))
            {
                var propertyName = Options.HeaderMap.FirstOrDefault(kvp => kvp.Value.Equals(headerTitle)).Key;
                var property = properties.FirstOrDefault(p => p.Name.Equals(propertyName));

                return property;
            }

            return properties.FirstOrDefault(p => p.Name.Equals(headerTitle));
        }
    }
}
