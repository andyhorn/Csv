using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csv.Core.Interfaces;

namespace Csv.Core.Models
{
    internal class Csv<T> : Csv, ICsv<T>
    {
        private readonly List<PropertyInfo> _properties;

        public Csv()
        {
            _properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType.IsPrimitive || p.PropertyType.Equals(typeof(string)))
                .OrderBy(p => p.Name)
                .ToList();

            Headers = new ICsvHeader[_properties.Count];
            for (var i = 0; i < Headers.Length; i++)
            {
                Headers[i] = new CsvHeader(i, _properties[i].Name);
            }

            HeaderMap = new Dictionary<PropertyInfo, string>();
            Ignores = new List<PropertyInfo>();
        }

        public Dictionary<PropertyInfo, string> HeaderMap { get; set; }

        public List<PropertyInfo> Ignores { get; set; }

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

        public T Get(int index)
        {
            throw new NotImplementedException();
        }

        public ICollection<T> Get()
        {
            throw new NotImplementedException();
        }

        public void Remove(int index)
        {
            throw new NotImplementedException();
        }

        private int GetColumnIndex(PropertyInfo property)
        {
            throw new NotImplementedException();
        }
    }
}
