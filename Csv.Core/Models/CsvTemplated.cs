﻿using System;
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

        public void Remove(int index)
        {
            if (index < 0 || index >= NumRows)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Cells = Cells
                .Select((c, i) => i != index ? c : null)
                .Where(c => c != null)
                .ToArray();
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
