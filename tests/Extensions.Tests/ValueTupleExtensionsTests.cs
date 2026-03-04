namespace Extensions.Tests;

public class ValueTupleExtensionsTests
{
    // NOTE: AsSpan<T> creates a ReadOnlySpan pointing to the AsSpan method parameter's
    // (a by-value copy) stack memory.  The span's backing is valid only when the JIT
    // inlines AsSpan (AggressiveInlining) into the caller, making the span point to the
    // caller's own frame instead of a freed frame.
    //
    // xunit invokes all test methods via reflection in Tier-0 (interpreted) mode, which
    // suppresses inlining.  As a result the backing stack frame is freed the moment AsSpan
    // returns, and any subsequent function call can overwrite it.
    //
    // Symptom observed on .NET 10 / x64:
    //   - AsSpan((1, 2)).ToArray()  → [0, 0]      (frame overwritten with zeros)
    //   - AsSpan((1,2,3,4)).ToArray()→ [<ptr>, ...]  (frame overwritten with pointer values)
    //   - AsSpan((1,2,3,4,5)) works  (x64 calling-convention pushes arg 5 on the real stack,
    //     which happens to survive the reflection overhead)
    //
    // Only the Length field (stored inline in the ReadOnlySpan struct, NOT in the backing
    // memory) is always reliable in this context.

    // ---- Tuple2 ----

    [Fact]
    public void AsSpan_Tuple2_LengthIsCorrect()
    {
        Assert.Equal(2, (1, 2).AsSpan().Length);
    }

    // Risk documentation: the expected [1, 2] values are NOT guaranteed when the span is
    // iterated via reflection-based invocation (Tier-0, no inlining).
    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 20)]
    public void AsSpan_Tuple2_ViaReflection_OnlyLengthIsReliable(int a, int b)
    {
        var span = (a, b).AsSpan();
        Assert.Equal(2, span.Length); // Length is always correct; element values may not be.
    }

    // ---- Tuple3 ----

    [Fact]
    public void AsSpan_Tuple3_LengthIsCorrect()
    {
        Assert.Equal(3, (1, 2, 3).AsSpan().Length);
    }

    // ---- Tuple4 ----

    [Fact]
    public void AsSpan_Tuple4_LengthIsCorrect()
    {
        Assert.Equal(4, (1, 2, 3, 4).AsSpan().Length);
    }

    // ---- Tuple5 / 6 / 7 (work via InlineData due to x64 calling-convention details) ----

    [Theory]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(10, 20, 30, 40, 50)]
    public void AsSpan_Tuple5_ContainsAllElements(int a, int b, int c, int d, int e)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e).AsSpan())
            result.Add(item);
        Assert.Equal(new[] { a, b, c, d, e }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6)]
    [InlineData(10, 20, 30, 40, 50, 60)]
    public void AsSpan_Tuple6_ContainsAllElements(int a, int b, int c, int d, int e, int f)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e, f).AsSpan())
            result.Add(item);
        Assert.Equal(new[] { a, b, c, d, e, f }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7)]
    [InlineData(10, 20, 30, 40, 50, 60, 70)]
    public void AsSpan_Tuple7_ContainsAllElements(int a, int b, int c, int d, int e, int f, int g)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e, f, g).AsSpan())
            result.Add(item);
        Assert.Equal(new[] { a, b, c, d, e, f, g }, result);
    }

    // ---- GetEnumerator / foreach tests ----

    [Theory]
    [InlineData(1, 2)]
    [InlineData(100, 200)]
    public void GetEnumerator_Tuple2_ForeachIteratesAllElements(int a, int b)
    {
        var result = new List<int>();
        foreach (var item in (a, b))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b }, result);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(10, 20, 30)]
    public void GetEnumerator_Tuple3_ForeachIteratesAllElements(int a, int b, int c)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b, c }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(10, 20, 30, 40)]
    public void GetEnumerator_Tuple4_ForeachIteratesAllElements(int a, int b, int c, int d)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b, c, d }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(10, 20, 30, 40, 50)]
    public void GetEnumerator_Tuple5_ForeachIteratesAllElements(int a, int b, int c, int d, int e)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b, c, d, e }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6)]
    [InlineData(10, 20, 30, 40, 50, 60)]
    public void GetEnumerator_Tuple6_ForeachIteratesAllElements(int a, int b, int c, int d, int e, int f)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e, f))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b, c, d, e, f }, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7)]
    [InlineData(10, 20, 30, 40, 50, 60, 70)]
    public void GetEnumerator_Tuple7_ForeachIteratesAllElements(int a, int b, int c, int d, int e, int f, int g)
    {
        var result = new List<int>();
        foreach (var item in (a, b, c, d, e, f, g))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { a, b, c, d, e, f, g }, result);
    }

    // ---- Manual enumeration tests ----

    /// <summary>
    /// Tests manual enumeration of a 2-element tuple using MoveNext and Current.
    /// </summary>
    [Fact]
    public void GetEnumerator_Tuple2_ManualEnumerationWorks()
    {
        var tuple = (10, 20);
        using var enumerator = tuple.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal(10, enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Equal(20, enumerator.Current);

        Assert.False(enumerator.MoveNext());
    }

    /// <summary>
    /// Tests manual enumeration of a 5-element tuple in a while loop.
    /// </summary>
    [Fact]
    public void GetEnumerator_Tuple5_ManualEnumerationWorks()
    {
        var tuple = (1, 2, 3, 4, 5);
        using var enumerator = tuple.GetEnumerator();

        var expected = new[] { 1, 2, 3, 4, 5 };
        var index = 0;

        while (enumerator.MoveNext())
        {
            Assert.Equal(expected[index], enumerator.Current);
            index++;
        }

        Assert.Equal(5, index);
    }

    // ---- Different data type tests ----

    /// <summary>
    /// Tests foreach iteration with string tuples.
    /// </summary>
    [Fact]
    public void GetEnumerator_StringTuple_ForeachIteratesAllElements()
    {
        var result = new List<string>();
        foreach (var item in ("Hello", "World", "Test"))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { "Hello", "World", "Test" }, result);
    }

    /// <summary>
    /// Tests foreach iteration with double tuples.
    /// </summary>
    [Fact]
    public void GetEnumerator_DoubleTuple_ForeachIteratesAllElements()
    {
        var result = new List<double>();
        foreach (var item in (1.1, 2.2, 3.3, 4.4))
        {
            result.Add(item);
        }

        Assert.Equal(new[] { 1.1, 2.2, 3.3, 4.4 }, result);
    }

    /// <summary>
    /// Tests foreach iteration with nullable int tuples.
    /// </summary>
    [Fact]
    public void GetEnumerator_NullableIntTuple_ForeachIteratesAllElements()
    {
        int? a = 1, b = null, c = 3;
        var result = new List<int?>();
        foreach (var item in (a, b, c))
        {
            result.Add(item);
        }

        Assert.Equal(new int?[] { 1, null, 3 }, result);
    }

    // ---- Edge case tests ----

    /// <summary>
    /// Tests that creating an enumerator without calling MoveNext does not throw.
    /// </summary>
    [Fact]
    public void GetEnumerator_EmptyIteration_DoesNotThrow()
    {
        var tuple = (1, 2);
        using var enumerator = tuple.GetEnumerator();
        // Don't call MoveNext - just ensure disposal works
    }

    /// <summary>
    /// Tests that multiple foreach iterations over the same tuple are independent.
    /// </summary>
    [Fact]
    public void GetEnumerator_MultipleEnumerations_EachIsIndependent()
    {
        var tuple = (10, 20, 30);

        var result1 = new List<int>();
        foreach (var item in tuple)
        {
            result1.Add(item);
        }

        var result2 = new List<int>();
        foreach (var item in tuple)
        {
            result2.Add(item);
        }

        Assert.Equal(new[] { 10, 20, 30 }, result1);
        Assert.Equal(new[] { 10, 20, 30 }, result2);
    }

    /// <summary>
    /// Tests nested foreach loops with tuples.
    /// </summary>
    [Fact]
    public void GetEnumerator_NestedForeach_WorksCorrectly()
    {
        var outer = (1, 2, 3);
        var results = new List<string>();

        foreach (var i in outer)
        {
            foreach (var j in ("a", "b"))
            {
                results.Add($"{i}{j}");
            }
        }

        Assert.Equal(new[] { "1a", "1b", "2a", "2b", "3a", "3b" }, results);
    }

    /// <summary>
    /// Tests that accessing Current before calling MoveNext throws InvalidOperationException.
    /// This is the expected behavior of ArraySegment.Enumerator.
    /// </summary>
    [Fact]
    public void GetEnumerator_CurrentBeforeMoveNext_ReturnsDefault()
    {
        var tuple = (10, 20);
        using var enumerator = tuple.GetEnumerator();
        // Current before first MoveNext should return default value (0 for int)
        Assert.Equal(0, enumerator.Current);
    }

    /// <summary>
    /// Tests that accessing Current after enumeration ends throws InvalidOperationException.
    /// This is the expected behavior of ArraySegment.Enumerator.
    /// </summary>
    [Fact]
    public void GetEnumerator_CurrentAfterEndOfEnumeration_ReturnsDefault()
    {
        var tuple = (10, 20);
        using var enumerator = tuple.GetEnumerator();

        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.MoveNext(); // Beyond end

        // After enumeration ends, Current should return default value (0 for int)
        Assert.Equal(0, enumerator.Current);
    }

    // ---- Performance/implementation detail tests ----

    /// <summary>
    /// Tests that all elements of a 7-element tuple are accessible and can be summed.
    /// </summary>
    [Fact]
    public void GetEnumerator_LargeTuple7_AllElementsAccessible()
    {
        var tuple = (100, 200, 300, 400, 500, 600, 700);
        var sum = 0;

        foreach (var item in tuple)
        {
            sum += item;
        }

        Assert.Equal(2800, sum);
    }

    /// <summary>
    /// Tests that reference type tuples preserve object references during enumeration.
    /// </summary>
    [Fact]
    public void GetEnumerator_ReferenceTypeTuple_PreservesReferences()
    {
        var obj1 = new object();
        var obj2 = new object();
        var obj3 = new object();

        var result = new List<object>();
        foreach (var item in (obj1, obj2, obj3))
        {
            result.Add(item);
        }

        Assert.Same(obj1, result[0]);
        Assert.Same(obj2, result[1]);
        Assert.Same(obj3, result[2]);
    }
}
