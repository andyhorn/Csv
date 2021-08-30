using System.Collections.Generic;
using System.Reflection;

namespace Csv.Core.Interfaces
{
    public interface ICsv<T> : ICsv
    {
        void Add(T item);

        void AddRange(ICollection<T> items);

        T Get(int index);

        ICollection<T> Get();

        void Remove(int index);

        Dictionary<PropertyInfo, string> HeaderMap { get; set; }

        List<PropertyInfo> Ignores { get; set; }
    }
}
