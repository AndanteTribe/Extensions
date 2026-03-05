[![dotnet-test](https://github.com/AndanteTribe/Extensions/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/AndanteTribe/Extensions/actions/workflows/dotnet-test.yml) [![nuget](https://img.shields.io/nuget/v/AndanteTribe.Extensions.svg)](https://www.nuget.org/packages/AndanteTribe.Extensions/) [![Releases](https://img.shields.io/github/release/AndanteTribe/Extensions.svg)](https://github.com/AndanteTribe/Extensions/releases) [![GitHub license](https://img.shields.io/github/license/AndanteTribe/Extensions.svg)](./LICENSE)

English | [日本語](./README_JA.md)

## Overview

**Extensions** is a lightweight C# library that provides practical, high-performance utility methods to make everyday development cleaner and more expressive.

It provides the following extension classes:

1. `CollectionExtensions` — Extensions for collections, `ArrayPool<T>`, and spans. Includes `AsSpan` for `List<T>`, `Grow` to resize pooled arrays, and `Rent` with a scoped `Handle<T>` for safe `ArrayPool<T>` usage.
2. `DisposableExtensions` — Extensions for `IDisposable`. Includes `AddTo` to assign a disposable to a container field.
3. `EnumExtensions` — High-performance bit-flag extensions for enums. Includes `HasBitFlags`, `ConstructFlags`, `AggregateFlags`, and `GetEnumerator` (foreach support).
4. `MemoryExtensions` — Extensions for `Memory<T>` and `ReadOnlyMemory<T>`. Enables direct foreach iteration without allocations.
5. `ValueTupleExtensions` — Extensions for homogeneous `ValueTuple`s (2–7 elements). Provides `AsSpan` and `GetEnumerator` (foreach support).

## Installation

### Requirements

This library requires .NET Standard 2.1 or higher.

### NuGet

The package can be obtained from NuGet.

#### .NET CLI

```sh
dotnet add package AndanteTribe.Extensions
```

#### Package Manager

```powershell
Install-Package AndanteTribe.Extensions
```

## API Reference

### CollectionExtensions

#### `List<T>.AsSpan()`

Converts a `List<T>` to a `Span<T>` without allocation using `CollectionsMarshal`.

```csharp
List<int> numbers = new List<int> { 10, 20, 30, 40, 50 };
Span<int> span = numbers.AsSpan();
foreach (var item in span)
{
    Console.WriteLine(item);
}
```

#### `ArrayPool<T>.Grow(ref T[] array, int minimumLength)`

Grows a pooled array if it is smaller than the required length. The old array is returned to the pool and a new, larger array is rented.

```csharp
var pool = ArrayPool<int>.Shared;
int[] buffer = pool.Rent(3);
try
{
    pool.Grow(ref buffer, 10);
    Console.WriteLine(buffer.Length); // >= 10
}
finally
{
    pool.Return(buffer);
}
```

#### `ArrayPool<T>.Rent(int minimumLength, out T[] array)`

Rents an array and returns a scoped `Handle<T>` that automatically returns the array to the pool when disposed. Designed for use with `using`.

```csharp
using var handle = ArrayPool<int>.Shared.Rent(10, out int[] array);
var span = array.AsSpan(0, 10);
for (int i = 0; i < span.Length; i++)
{
    span[i] = i;
}
// array is returned to the pool when handle is disposed
```

---

### DisposableExtensions

#### `IDisposable.AddTo<T>(ref T disposableContainer)`

Assigns an `IDisposable` instance to a container field. Useful for managing resource lifetimes with a single field.

```csharp
private IDisposable _resource;

public void Initialize()
{
    CreateManagedResource().AddTo(ref _resource);
}

public void Cleanup()
{
    _resource?.Dispose();
}
```

---

### EnumExtensions

All enum extension methods operate on any enum type `T` where `T : struct, Enum` and are implemented without boxing using `Unsafe`. The `GetEnumerator<T>(this T value)` method currently assumes the enum's underlying type is `int` and should only be used with such enums.

#### `HasBitFlags<T>(T flag)`

Determines whether all bits in `flag` are set in the enum value.

```csharp
[Flags]
public enum FileAccess { None = 0, Read = 1, Write = 2, Execute = 4, ReadWrite = Read | Write }

FileAccess perms = FileAccess.Read | FileAccess.Write;
bool hasRead = perms.HasBitFlags(FileAccess.Read);    // true
bool hasExec = perms.HasBitFlags(FileAccess.Execute); // false
```

#### `ConstructFlags<T>()`

Returns `true` if the value has exactly one bit flag set (i.e., is a power of two and non-zero).

```csharp
Console.WriteLine(FileAccess.Read.ConstructFlags());      // true
Console.WriteLine(FileAccess.ReadWrite.ConstructFlags()); // false (two flags)
Console.WriteLine(FileAccess.None.ConstructFlags());      // false (zero)
```

#### `AggregateFlags<T>(Span<T>)` / `AggregateFlags<T>(ReadOnlySpan<T>)`

Combines multiple flags from a span into a single enum value using bitwise OR.

```csharp
var flags = new[] { FileAccess.Read, FileAccess.Write };
FileAccess combined = flags.AsSpan().AggregateFlags(); // FileAccess.ReadWrite
```

#### `GetEnumerator<T>()` — foreach over enum flags

Enables foreach iteration over each individual bit flag set in an enum value.

```csharp
FileAccess perms = FileAccess.Read | FileAccess.Write;
foreach (var flag in perms)
{
    Console.WriteLine(flag); // Read, Write
}
```

---

### MemoryExtensions

#### `Memory<T>.GetEnumerator()` / `ReadOnlyMemory<T>.GetEnumerator()`

Enables foreach iteration over `Memory<T>` and `ReadOnlyMemory<T>` directly, without calling `.Span` explicitly.

```csharp
ReadOnlyMemory<char> memory = "Hello".AsMemory();
foreach (var c in memory)
{
    Console.WriteLine(c); // H, e, l, l, o
}
```

---

### ValueTupleExtensions

Supports homogeneous tuples with 2 to 7 elements of the same type `T`.

#### `(T, T, ...).AsSpan()`

Converts a homogeneous `ValueTuple` to a `ReadOnlySpan<T>` without allocation. Because `AsSpan()` uses a `ref this` receiver, you must assign the tuple to a local variable before calling it.

```csharp
var tuple = ("Hello", "World");
var span = tuple.AsSpan();
foreach (var item in span)
{
    Console.WriteLine(item); // Hello, World
}
```

#### `(T, T, ...).GetEnumerator()` — foreach over tuples

Enables foreach iteration over a homogeneous `ValueTuple`. The enumerator uses `ArrayPool<T>` internally and is automatically disposed at the end of a foreach loop.

```csharp
foreach (var num in (10, 20, 30))
{
    Console.WriteLine(num); // 10, 20, 30
}
```

## License

This library is licensed under the [MIT License](./LICENSE).
