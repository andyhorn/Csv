using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    /// <summary>
    /// Implements the <see cref="ICsv{T}"/> interface by extending the
    /// <see cref="Csv"/> class with templated methods.
    /// </summary>
    /// <typeparam name="T">The data <see cref="Type"/> being held by this class.</typeparam>
    internal class Csv<T> : Csv, ICsv<T>
    {
        private readonly List<PropertyInfo> _properties;
        private readonly Dictionary<PropertyInfo, string> _headerMap;
        private List<PropertyInfo> _ignores;

        /// <summary>
        /// Initializes a new instance of the <see cref="Csv{T}"/> class.
        /// </summary>
        public Csv(List<PropertyInfo> ignores = null, Dictionary<PropertyInfo, string> headerMap = null)
        {
            if (ignores != null && ignores.Any())
            {
                foreach (var property in ignores)
                {
                    if (!property.DeclaringType.Equals(typeof(T)))
                    {
                        throw new ArgumentException($"Parameter 'ignores' can only contain properties that belong to type parameter {typeof(T)}.");
                    }

                    if (!property.PropertyType.IsPrimitive && !property.PropertyType.Equals(typeof(string)))
                    {
                        throw new ArgumentException($"Parameter 'ignores' can only contain primitive property types.");
                    }
                }
            }

            if (headerMap != null && headerMap.Any())
            {
                foreach (var (property, _) in headerMap)
                {
                    if (!property.DeclaringType.Equals(typeof(T)))
                    {
                        throw new ArgumentException($"Parameter 'headerMap' can only contain properties that belong to type parameter {typeof(T)}.");
                    }

                    if (!property.PropertyType.IsPrimitive && !property.PropertyType.Equals(typeof(string)))
                    {
                        throw new ArgumentException($"Parameter 'headerMap' can only contain primitive property types.");
                    }
                }
            }

            _properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType.IsPrimitive || p.PropertyType.Equals(typeof(string)))
                .OrderBy(p => p.Name)
                .ToList();

            _headerMap = headerMap ?? new Dictionary<PropertyInfo, string>();
            _ignores = ignores ?? new List<PropertyInfo>();

            var headers = new List<ICsvHeader>();
            var index = 0;
            foreach (var property in _properties)
            {
                if (_ignores.Contains(property))
                {
                    continue;
                }

                var title = _headerMap.ContainsKey(property)
                    ? _headerMap[property]
                    : property.Name;

                var header = new CsvHeader(index++, title);

                headers.Add(header);
            }

            Headers = headers.ToArray();
        }

        /// <inheritdoc/>
        public ImmutableDictionary<PropertyInfo, string> HeaderMap
            => ImmutableDictionary.CreateRange(_headerMap);

        /// <inheritdoc/>
        public ImmutableList<PropertyInfo> Ignores
            => ImmutableList.CreateRange(_ignores);

        /// <inheritdoc//>
        public void AddHeaderMap(PropertyInfo property, string title)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (!property.DeclaringType.Equals(typeof(T)))
            {
                throw new ArgumentException($"Property {property} must belong to type parameter {typeof(T)}");
            }

            if (_headerMap.ContainsKey(property))
            {
                _headerMap.Remove(property);
            }

            _headerMap.Add(property, title);

            var header = Headers.FirstOrDefault(h => h.Title.Equals(property.Name));

            if (header == null)
            {
                return;
            }

            var index = header.Index;
            Headers[index] = new CsvHeader(index, title);
        }

        /// <inheritdoc/>
        public void RemoveHeaderMap(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (!property.DeclaringType.Equals(typeof(T)))
            {
                throw new ArgumentException($"Property {property} must belong to type parameter {typeof(T)}");
            }

            if (!_headerMap.ContainsKey(property))
            {
                return;
            }

            var title = _headerMap[property];
            var header = Headers.FirstOrDefault(h => h.Title.Equals(title));

            if (header == null)
            {
                return;
            }

            var index = header.Index;
            Headers[index] = new CsvHeader(index, property.Name);
            _headerMap.Remove(property);
        }

        /// <inheritdoc/>
        public void IgnoreProperty(PropertyInfo property)
        {
            // if the property is already in the list, do nothing
            if (_ignores.Contains(property))
            {
                return;
            }

            // if the property does not belong to this CSV's type, throw
            if (!property.DeclaringType.Equals(typeof(T)))
            {
                throw new ArgumentException(nameof(property));
            }

            // add the property
            _ignores.Add(property);

            // remove the property column
            var index = Headers.FirstOrDefault(h => h.Title.Equals(property.Name)).Index;
            for (var row = 0; row < NumRows; row++)
            {
                Cells[row] = Cells[row].Where((c, i) => i != index).ToArray();
            }

            // remove the header
            Headers = Headers.Where((h, i) => i != index).ToArray();
        }

        /// <inheritdoc/>
        public void AcknowledgeProperty(PropertyInfo property)
        {
            // if the property is not being ignored, do nothing
            if (!_ignores.Contains(property))
            {
                return;
            }

            // if the property does not belong to the CSV's type, throw
            if (!property.DeclaringType.Equals(typeof(T)))
            {
                throw new ArgumentException(nameof(property));
            }

            // remove the property
            _ignores.Remove(property);

            // add a header
            Headers = Headers
                .Append(new CsvHeader(Headers.Count(), property.Name))
                .ToArray();
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var rowIndex = NumRows;

            foreach (var property in _properties)
            {
                if (Ignores?.Contains(property) ?? false)
                {
                    continue;
                }

                object value = property.GetValue(item);
                var columnIndex = GetColumnIndex(property);
                SetCell(rowIndex, columnIndex, value);
            }
        }

        /// <inheritdoc/>
        public void AddRange(ICollection<T> items)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                Add(item);
            }
        }

        /// <inheritdoc/>
        public T Get(int index)
        {
            if (index < 0 || index >= NumRows)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var item = Activator.CreateInstance<T>();

            foreach (var property in _properties)
            {
                var columnIndex = GetColumnIndex(property);
                object value = GetCell(index, columnIndex).Value;
                property.SetValue(item, value);
            }

            return item;
        }

        /// <inheritdoc/>
        public ICollection<T> Get()
        {
            var items = new List<T>();

            for (var i = 0; i < NumRows; i++)
            {
                var item = Get(i);
                items.Add(item);
            }

            return items;
        }

        /// <inheritdoc/>
        public void Remove(int index)
        {
            if (index < 0 || index >= NumRows)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Cells = Cells.Where((c, i) => i != index).ToArray();
        }

        private int GetColumnIndex(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var header = FindOrCreateHeader(property);

            return header.Index;
        }

        private ICsvHeader FindOrCreateHeader(PropertyInfo property)
        {
            var title = property.Name;

            if (HeaderMap?.ContainsKey(property) ?? false)
            {
                title = HeaderMap[property];
            }

            var header = Headers.FirstOrDefault(h => h.Title.Equals(title));

            if (header == null)
            {
                header = new CsvHeader(NumColumns, title);
                Headers = Headers.Append(header).ToArray();
            }

            return header;
        }
    }
}
