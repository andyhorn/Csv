using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Csv.Core.Interfaces
{
    /// <summary>
    /// Defines a templated CSV document for mapping between CSV-formatted data
    /// and user-defined C# classes.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of data being stored.</typeparam>
    public interface ICsv<T> : ICsv
        where T : class
    {
        /// <summary>
        /// Gets or sets a mapping of <see cref="PropertyInfo"/> to strings.
        /// The value will be used as the header instead of the property name.
        /// </summary>
        ImmutableDictionary<PropertyInfo, string> HeaderMap { get; }

        /// <summary>
        /// Gets or sets a list of <see cref="PropertyInfo"/> to be ignored when
        /// storing, reading, and writing the CSV document data.
        /// </summary>
        ImmutableList<PropertyInfo> Ignores { get; }

        /// <summary>
        /// Adds a new header mapping to the dictionary. The <see cref="PropertyInfo"/>
        /// column will use the new title when writing the CSV data.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="title">The new title for the column.</param>
        void AddHeaderMap(PropertyInfo property, string title);

        /// <summary>
        /// Removes an existing mapping for the <see cref="PropertyInfo"/>,
        /// returning the column title to the property name.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to be un-mapped.</param>
        void RemoveHeaderMap(PropertyInfo property);

        /// <summary>
        /// Adds a <see cref="PropertyInfo"/> to the <see cref="Ignores"/> list.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to be ignored.</param>
        void IgnoreProperty(PropertyInfo property);

        /// <summary>
        /// Removes a <see cref="PropertyInfo"/> from the <see cref="Ignores"/> list.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to be acknowledged.</param>
        void AcknowledgeProperty(PropertyInfo property);

        /// <summary>
        /// Adds a new item of type <typeparamref name="T"/> to the CSV document.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        void Add(T item);

        /// <summary>
        /// Adds a collection of type <typeparamref name="T"/> to the CSV document.
        /// </summary>
        /// <param name="items">An <see cref="ICollection{T}"/> of type
        /// <typeparamref name="T"/>.</param>
        void AddRange(ICollection<T> items);

        /// <summary>
        /// Gets an object of type <typeparamref name="T"/> from the CSV document
        /// at the specified index. Throws an <see cref="ArgumentOutOfRangeException"/>
        /// if the index is invalid.
        /// </summary>
        /// <param name="index">The index at which to retrieve the object data.</param>
        /// <returns>An object of type <typeparamref name="T"/>.</returns>
        T Get(int index);

        /// <summary>
        /// Gets a collection of objects of type <typeparamref name="T"/>
        /// containing every item in the CSV document.
        /// </summary>
        /// <returns></returns>
        ICollection<T> Get();

        /// <summary>
        /// Removes an object from the CSV collection at the specified index (row).
        /// </summary>
        /// <param name="index">The index (row) to be removed.</param>
        void Remove(int index);
    }
}
