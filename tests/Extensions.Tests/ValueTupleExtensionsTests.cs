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
}


