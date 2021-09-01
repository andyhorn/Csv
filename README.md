# Csv.Core 2.0

A simple .Net Core class library for reading, manipulating, and writing CSV-formatted data and, as of 2.0, mapping between CSV data and user-defined classes.

## v2.0.0 Release Notes

**New Features**

  * A new `ICsv<T>` interface supports templates and mapping between CSV and user-defined classes

**Breaking Changes**

  * The reader and writer classes are no longer static and must be instantiated with the `new` keyword
  * The reader and writer classes are now implementations of the `ICsvReader` and `ICsvWriter` interfaces, respectively
  * The `CsvFactory`'s "New" property has been depricated in favor of a traditional method call: `CsvFactory.New();`

# Overview

Read, Manipulate, and Write CSV-formatted data with the base `ICsv` interface or the type-specific `ICsv<T>` interface.

## ICsv

The base `ICsv` provides an interface for managing raw CSV data using a set of **Headers**, **Rows**, **Columns**, and **Cells**.

![Table Example](Csv.Core.png)

The `ICsv` interface exposes properties for each of these elements.

Element|Property|Type|Comments
---|---|---|---
Header|`ICsv.Headers`|`ICsvHeader[]`|Contains a `Title` and `Index`
Row|`ICsv.Rows`|`ICsvRow[]`|Contains an `ICsvCell[]` for all `ICsvCell` objects in the row
Column|`ICsv.Columns`|`ICsvColumn[]`|Contains an `ICsvCell[]` property for all `ICsvCell` objects in the column
Cell|`ICsv.Cells`|`ICsvCell[][]`|<ul><li>[Row][Column] indexing</li><li>Contains a reference to the cell's `ICsvHeader`</li></ul>


### The ICsv Interface
The base component of this library is the `ICsv` interface.

```
public interface ICsv
{
    string Filename { get; set; }
    bool HasHeaders { get; }
    char Separator { get; set; }
    int NumRows { get; }
    int NumColumns { get; }

    ICsvHeader[] Headers { get; set; }
    ICsvRow[] Rows { get; }
    ICsvColumn[] Columns { get; }
    ICsvCell[][] Cells { get; } // uses row/column indexing
    ICsvCell GetCell(int row, int column);
    void SetCell(int row, int column, object value); // will create rows and/or columns as needed
}
```

### The ICsv\<T> Interface
New in v2.0 is the templated `ICsv<T>` interface, which extends the base `ICsv` interface with new class/object-focused methods:

```
public interface ICsv<T> : ICsv
	where T: class
{
	// change the 'title' used in the header row for specific class properties
	ImmutableDictionary<PropertyInfo, string> HeaderMap { get; }
	
	// ignore entire properties (columns) in the CSV
	ImmutableList<PropertyInfo> Ignores { get; }
	
	// add a new property <--> title mapping
	void AddHeaderMap(PropertyInfo property, string title);
	
	// remove an existing property <--> title map
	void RemoveHeaderMap(PropertyInfo property);
	
	// add a property to the ignore list
	void IgnoreProperty(PropertyInfo property);
	
	// remove a property from the ignore list
	void AcknowledgeProperty(PropertyInfo property);
	
	// add a new item (row) of type T to the CSV
	void Add(T item);
	
	// add a collection of items to the CSV
	void AddRange(ICollection<T> items);
	
	// retrieve an item from the specified row index
	T Get(int index);
	
	// retrieve all items from the CSV
	ICollection<T> Get();
	
	// remove an item (row) from the CSV
	void Remove(int index);
}	
```

**Note:** At this time, this interface only supports reading/writing primitive data types. Complex data structures will not be saved in the CSV data.

## Factories

Currently, the implementations of the `ICsv` and `ICsv<T>` interfaces are marked `internal` and cannot be instantiated directly; instead, you must use a reader class to create an instance and fill it with data OR a factory to retrieve a new, empty instance.

For a base `ICsv` with no template mapping capabilities, use:

```
ICsv csv = CsvFactory.New();
```

For an instance of the templated `ICsv<T>` interface, use:

```
ICsv<ClassType> csv = CsvFactory.ForType<ClassType>();
```

# Usage

## Reading

Within the Csv.Core.Readers namespace are two `CsvReader` classes that will read CSV data and return an `ICsv` (or `ICsv<T>`) instance.

**ICsv**

```
var filename = GetFileName();
ICsvReader reader = new CsvReader();
ICsv csv = reader.FromFile(filename);
```

**ICsv\<T>**

```
var filename = GetFileName();
ICsvReader reader = new CsvReader<ClassType>();
ICsv<ClassType> csv = reader.FromFile(filename);
```

### The ICsvReader Interface

The `ICsvReader` interface exposes four different types of read:

```
public interface ICsvReader
{
	bool HasHeaders { get; set; }
	char Separator { get; set; }
	
	ICsv FromString(string csvData);
	ICsv FromLines(string[] lines);
	ICsv FromFile(string filePath);
	ICsv FromStream(Stream stream);
}
```

Both implementations of the interface use default values of:

  * `HasHeaders = true`
  * `Separator = ','`

## Writing

Within the Csv.Core.Writers namespace are two `ICsvWriter` implementations that will take an `ICsv` instance and export the data in CSV format.

**ICsv**

```
ICsv csv = MakeCsv();
ICsvWriter writer = new CsvWriter();
writer.ToFile(csv, "/path/to/file.csv");
```

**ICsv\<T>**

```
ICsv<ClassType> csv = MakeCsv();
ICsvWriter writer = new CsvWriter<ClassType>();
writer.ToFile(csv, "/path/to/file.csv");
```

**Note:** The non-templated writer _can_ write `ICsv<T>` objects, but it will not use the `HeaderMap` or `Ignores` properties.

### The ICsvWriter Interface

The `ICsvWriter` interface exposes several methods for exporting CSV data:

```
public ICsvWriter
{
	Task ToFileAsync(ICsv csv, string filePath);
	void ToFile(ICsv csv, string filePath);
	Task ToStreamAsync(ICsv csv, Stream stream);
	void ToStream(ICsv csv, Stream stream);
	Task<string> ToStringAsync(ICsv csv);
	string ToString(ICsv csv);
}
```

# Future Objectives

Possible improvements:

  * <s>Templating for class mapping</s> - Implemented in v2.0.0

# Known Bugs

  * None