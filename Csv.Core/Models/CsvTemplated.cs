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
        }

        public Dictionary<PropertyInfo, string> HeaderMap { get; set; }

        public List<PropertyInfo> Ignores { get; set; }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void AddRange(ICollection<T> items)
        {
            throw new NotImplementedException();
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
    }
}
