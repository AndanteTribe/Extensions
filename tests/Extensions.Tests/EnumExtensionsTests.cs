namespace Extensions.Tests;

[Flags]
public enum ByteEnum : byte
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
}

[Flags]
public enum ShortEnum : short
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
}

[Flags]
public enum IntEnum : int
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
}

[Flags]
public enum LongEnum : long
{
    None = 0,
    A = 1L << 0,
    B = 1L << 1,
    C = 1L << 2,
    HighBit = 1L << 32,
}

public class EnumExtensionsTests
{
    // ---------- HasBitFlags ----------

    [Theory]
    [InlineData(ByteEnum.A | ByteEnum.B, ByteEnum.A, true)]
    [InlineData(ByteEnum.A | ByteEnum.B, ByteEnum.B, true)]
    [InlineData(ByteEnum.A | ByteEnum.B, ByteEnum.A | ByteEnum.B, true)]
    [InlineData(ByteEnum.A, ByteEnum.B, false)]
    public void HasBitFlags_Byte(ByteEnum value, ByteEnum flag, bool expected)
        => Assert.Equal(expected, value.HasBitFlags(flag));

    [Theory]
    [InlineData(ShortEnum.A | ShortEnum.B, ShortEnum.A, true)]
    [InlineData(ShortEnum.A, ShortEnum.B, false)]
    public void HasBitFlags_Short(ShortEnum value, ShortEnum flag, bool expected)
        => Assert.Equal(expected, value.HasBitFlags(flag));

    [Theory]
    [InlineData(IntEnum.A | IntEnum.B, IntEnum.A, true)]
    [InlineData(IntEnum.A, IntEnum.B, false)]
    public void HasBitFlags_Int(IntEnum value, IntEnum flag, bool expected)
        => Assert.Equal(expected, value.HasBitFlags(flag));

    [Theory]
    [InlineData(LongEnum.A | LongEnum.B, LongEnum.A, true)]
    [InlineData(LongEnum.A, LongEnum.B, false)]
    public void HasBitFlags_Long(LongEnum value, LongEnum flag, bool expected)
        => Assert.Equal(expected, value.HasBitFlags(flag));

    // ---------- ConstructFlags ----------

    [Theory]
    [InlineData(ByteEnum.A, true)]
    [InlineData(ByteEnum.None, false)]
    [InlineData(ByteEnum.A | ByteEnum.B, false)]
    public void ConstructFlags_Byte(ByteEnum value, bool expected)
        => Assert.Equal(expected, value.ConstructFlags());

    [Theory]
    [InlineData(ShortEnum.A, true)]
    [InlineData(ShortEnum.None, false)]
    [InlineData(ShortEnum.A | ShortEnum.B, false)]
    public void ConstructFlags_Short(ShortEnum value, bool expected)
        => Assert.Equal(expected, value.ConstructFlags());

    [Theory]
    [InlineData(IntEnum.A, true)]
    [InlineData(IntEnum.None, false)]
    [InlineData(IntEnum.A | IntEnum.B, false)]
    public void ConstructFlags_Int(IntEnum value, bool expected)
        => Assert.Equal(expected, value.ConstructFlags());

    [Theory]
    [InlineData(LongEnum.A, true)]
    [InlineData(LongEnum.None, false)]
    [InlineData(LongEnum.A | LongEnum.B, false)]
    public void ConstructFlags_Long(LongEnum value, bool expected)
        => Assert.Equal(expected, value.ConstructFlags());

    // ---------- AggregateFlags ----------

    [Fact]
    public void AggregateFlags_Span_Byte_Empty_ReturnsDefault()
    {
        // Also covers the Span<T> overload (delegates to ReadOnlySpan<T>).
        var result = Span<ByteEnum>.Empty.AggregateFlags();
        Assert.Equal(ByteEnum.None, result);
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Short_Empty_ReturnsDefault()
    {
        Assert.Equal(ShortEnum.None, ReadOnlySpan<ShortEnum>.Empty.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Int_Empty_ReturnsDefault()
    {
        Assert.Equal(IntEnum.None, ReadOnlySpan<IntEnum>.Empty.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Long_Empty_ReturnsDefault()
    {
        Assert.Equal(LongEnum.None, ReadOnlySpan<LongEnum>.Empty.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Byte_ReturnsOrResult()
    {
        ReadOnlySpan<ByteEnum> flags = new ByteEnum[] { ByteEnum.A, ByteEnum.C };
        Assert.Equal(ByteEnum.A | ByteEnum.C, flags.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Short_ReturnsOrResult()
    {
        ReadOnlySpan<ShortEnum> flags = new ShortEnum[] { ShortEnum.A, ShortEnum.B };
        Assert.Equal(ShortEnum.A | ShortEnum.B, flags.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Int_ReturnsOrResult()
    {
        ReadOnlySpan<IntEnum> flags = new IntEnum[] { IntEnum.A, IntEnum.B };
        Assert.Equal(IntEnum.A | IntEnum.B, flags.AggregateFlags());
    }

    [Fact]
    public void AggregateFlags_ReadOnlySpan_Long_ReturnsOrResult()
    {
        ReadOnlySpan<LongEnum> flags = new LongEnum[] { LongEnum.A, LongEnum.B };
        Assert.Equal(LongEnum.A | LongEnum.B, flags.AggregateFlags());
    }

    // ---------- GetEnumerator / Enumerator ----------

    [Fact]
    public void GetEnumerator_IntEnum_MultipleFlagsAreEnumerated()
    {
        var result = new List<IntEnum>();
        foreach (var flag in IntEnum.A | IntEnum.B | IntEnum.C)
            result.Add(flag);
        Assert.Equal(new[] { IntEnum.A, IntEnum.B, IntEnum.C }, result);
    }

    [Fact]
    public void GetEnumerator_IntEnum_None_YieldsNothing()
    {
        var result = new List<IntEnum>();
        foreach (var flag in IntEnum.None)
            result.Add(flag);
        Assert.Empty(result);
    }

    // Risk case: Enumerator<T> constructor reads 4 bytes via Unsafe.As<T, int>(ref value).
    // For ByteEnum (1 byte), this reads 3 bytes beyond the enum value's bounds.
    // On .NET with zero-initialized stacks the upper bytes are typically 0, so simple values
    // behave correctly in practice — but the code relies on undefined behavior.
    [Fact]
    public void GetEnumerator_ByteEnum_SingleFlag_DocumentedBehavior()
    {
        var result = new List<ByteEnum>();
        foreach (var flag in ByteEnum.A)
            result.Add(flag);
        Assert.Contains(ByteEnum.A, result);
    }

    // Risk case: Enumerator<T> stores the value in an int field (Unsafe.As<T, int>).
    // For LongEnum (8 bytes), only the lower 32 bits are captured on little-endian systems.
    // Flags above bit 31 (e.g. HighBit = 1L << 32) are silently dropped.
    [Fact]
    public void GetEnumerator_LongEnum_HighBitFlag_IsNotEnumeratedDueToIntTruncation()
    {
        var result = new List<LongEnum>();
        foreach (var flag in LongEnum.HighBit)
            result.Add(flag);
        // On little-endian: _value = lower 32 bits of 0x0000_0001_0000_0000 = 0 → nothing enumerated.
        Assert.DoesNotContain(LongEnum.HighBit, result);
    }
}
