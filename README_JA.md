[![dotnet-test](https://github.com/AndanteTribe/Extensions/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/AndanteTribe/Extensions/actions/workflows/dotnet-test.yml) [![nuget](https://img.shields.io/nuget/v/AndanteTribe.Extensions.svg)](https://www.nuget.org/packages/AndanteTribe.Extensions/) [![Releases](https://img.shields.io/github/release/AndanteTribe/Extensions.svg)](https://github.com/AndanteTribe/Extensions/releases) [![GitHub license](https://img.shields.io/github/license/AndanteTribe/Extensions.svg)](./LICENSE)

[English](./README.md) | 日本語

## 概要

**Extensions** は、日々の開発をよりクリーンで表現豊かにするための、実用的かつ高パフォーマンスなユーティリティメソッドを提供する軽量な C# ライブラリです。

以下の拡張クラスを提供します。

1. `CollectionExtensions` — コレクション・`ArrayPool<T>`・スパン向けの拡張メソッドです。`List<T>` の `AsSpan`、プールされた配列のリサイズ用 `Grow`、安全な `ArrayPool<T>` 利用のためのスコープ付き `Handle<T>` を返す `Rent` を含みます。
2. `DisposableExtensions` — `IDisposable` 向けの拡張メソッドです。コンテナフィールドへの割り当てを行う `AddTo` を含みます。
3. `EnumExtensions` — enum 向けの高パフォーマンスなビットフラグ拡張メソッドです。`HasBitFlags`、`ConstructFlags`、`AggregateFlags`、`GetEnumerator`（foreach サポート）を含みます。ボクシングなしで `Unsafe` を用いて実装されています。
4. `MemoryExtensions` — `Memory<T>` および `ReadOnlyMemory<T>` 向けの拡張メソッドです。アロケーションなしで直接 foreach 反復を可能にします。
5. `ValueTupleExtensions` — 同型の `ValueTuple`（2〜7 要素）向けの拡張メソッドです。`AsSpan` および `GetEnumerator`（foreach サポート）を提供します。

## インストール

### 必要条件

このライブラリは .NET Standard 2.1 以上が必要です。

### NuGet

パッケージは NuGet から取得できます。

#### .NET CLI

```sh
dotnet add package AndanteTribe.Extensions
```

#### パッケージマネージャー

```powershell
Install-Package AndanteTribe.Extensions
```

## API リファレンス

### CollectionExtensions

#### `List<T>.AsSpan()`

`CollectionsMarshal` を使用して、アロケーションなしで `List<T>` を `Span<T>` に変換します。

```csharp
List<int> numbers = new List<int> { 10, 20, 30, 40, 50 };
Span<int> span = numbers.AsSpan();
foreach (var item in span)
{
    Console.WriteLine(item);
}
```

#### `ArrayPool<T>.Grow(ref T[] array, int minimumLength)`

プールされた配列が必要なサイズより小さい場合に拡張します。古い配列はプールに返却され、新しい大きな配列がレンタルされます。

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

配列をレンタルし、`using` ステートメントで使用できるスコープ付き `Handle<T>` を返します。スコープを抜けると自動的にプールへ返却されます。

```csharp
using var handle = ArrayPool<int>.Shared.Rent(10, out int[] array);
var span = array.AsSpan(0, 10);
for (int i = 0; i < span.Length; i++)
{
    span[i] = i;
}
// スコープを抜けると handle が Dispose され、配列がプールに返却される
```

---

### DisposableExtensions

#### `IDisposable.AddTo<T>(ref T disposableContainer)`

`IDisposable` インスタンスをコンテナフィールドに割り当てます。単一フィールドでリソースのライフタイムを管理する際に便利です。

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

すべての enum 拡張メソッドは `T : struct, Enum` の制約を持つ enum 型で動作し、`Unsafe` を使用してボクシングなしに実装されています。ただし、`GetEnumerator<T>(this T value)` は内部的に値を `int` として解釈するため、基底型が `int` の enum にのみ対応します。

#### `HasBitFlags<T>(T flag)`

`flag` のすべてのビットが enum 値に設定されているかどうかを確認します。

```csharp
[Flags]
public enum FileAccess { None = 0, Read = 1, Write = 2, Execute = 4, ReadWrite = Read | Write }

FileAccess perms = FileAccess.Read | FileAccess.Write;
bool hasRead = perms.HasBitFlags(FileAccess.Read);    // true
bool hasExec = perms.HasBitFlags(FileAccess.Execute); // false
```

#### `ConstructFlags<T>()`

値にビットフラグが 1 つだけ設定されている場合（つまり 2 の冪乗かつ非ゼロの場合）に `true` を返します。

```csharp
Console.WriteLine(FileAccess.Read.ConstructFlags());      // true
Console.WriteLine(FileAccess.ReadWrite.ConstructFlags()); // false（2 つのフラグ）
Console.WriteLine(FileAccess.None.ConstructFlags());      // false（ゼロ）
```

#### `AggregateFlags<T>(Span<T>)` / `AggregateFlags<T>(ReadOnlySpan<T>)`

スパンの複数のフラグをビット OR で 1 つの enum 値にまとめます。

```csharp
var flags = new[] { FileAccess.Read, FileAccess.Write };
FileAccess combined = flags.AsSpan().AggregateFlags(); // FileAccess.ReadWrite
```

#### `GetEnumerator<T>()` — enum フラグの foreach

enum 値に設定された各ビットフラグを foreach で反復できるようにします。

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

`Memory<T>` および `ReadOnlyMemory<T>` を `.Span` を呼び出さずに直接 foreach で反復できるようにします。

```csharp
ReadOnlyMemory<char> memory = "Hello".AsMemory();
foreach (var c in memory)
{
    Console.WriteLine(c); // H, e, l, l, o
}
```

---

### ValueTupleExtensions

同じ型 `T` の要素を 2〜7 個持つ同型 `ValueTuple` をサポートします。

#### `(T, T, ...).AsSpan()`

同型の `ValueTuple` をアロケーションなしで `ReadOnlySpan<T>` に変換します。`AsSpan()` は `ref this` レシーバーを使用するため、タプルリテラルに直接呼び出すことはできません。事前にローカル変数に代入する必要があります。

```csharp
var tuple = ("Hello", "World");
var span = tuple.AsSpan();
foreach (var item in span)
{
    Console.WriteLine(item); // Hello, World
}
```

#### `(T, T, ...).GetEnumerator()` — タプルの foreach

同型の `ValueTuple` を foreach で反復できるようにします。列挙子は内部で `ArrayPool<T>` を使用し、foreach ループ終了時に自動的に破棄されます。

```csharp
foreach (var num in (10, 20, 30))
{
    Console.WriteLine(num); // 10, 20, 30
}
```

## ライセンス

このライブラリは [MIT ライセンス](./LICENSE) のもとで公開されています。
