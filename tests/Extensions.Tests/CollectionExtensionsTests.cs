using System.Buffers;

namespace Extensions.Tests;

public class CollectionExtensionsTests
{
    [Fact]
    public void AsSpan_ReturnsCorrectElements()
    {
        var list = new List<int> { 1, 2, 3 };
        var span = list.AsSpan();
        Assert.Equal(3, span.Length);
        Assert.Equal(1, span[0]);
        Assert.Equal(2, span[1]);
        Assert.Equal(3, span[2]);
    }

    [Fact]
    public void AsSpan_EmptyList_ReturnsEmptySpan()
    {
        var list = new List<int>();
        var span = list.AsSpan();
        Assert.Equal(0, span.Length);
    }

    [Fact]
    public void Grow_WhenArrayLargeEnough_DoesNotGrow()
    {
        var pool = ArrayPool<int>.Shared;
        var array = pool.Rent(10);
        var original = array;
        try
        {
            pool.Grow(ref array, 5);
            Assert.Same(original, array);
        }
        finally
        {
            pool.Return(array);
        }
    }

    [Fact]
    public void Grow_WhenArrayTooSmall_WithExistingData_GrowsAndCopies()
    {
        var pool = ArrayPool<int>.Shared;
        var array = pool.Rent(3);
        // Use array.Length + 1 to guarantee the grow condition triggers,
        // regardless of the actual length returned by the pool.
        var requiredLength = array.Length + 1;
        array[0] = 42;
        var original = array;
        try
        {
            pool.Grow(ref array, requiredLength);
            Assert.True(array.Length >= requiredLength);
            Assert.NotSame(original, array);
            Assert.Equal(42, array[0]);
        }
        finally
        {
            pool.Return(array);
        }
    }

    [Fact]
    public void Grow_WhenEmptyArray_GrowsWithoutCopy()
    {
        var pool = ArrayPool<int>.Shared;
        var array = Array.Empty<int>();
        pool.Grow(ref array, 5);
        try
        {
            Assert.True(array.Length >= 5);
        }
        finally
        {
            pool.Return(array);
        }
    }

    [Fact]
    public void Rent_ReturnsHandleAndArray()
    {
        var pool = ArrayPool<int>.Shared;
        using var handle = pool.Rent(10, out var array);
        Assert.NotNull(array);
        Assert.True(array.Length >= 10);
    }
}
