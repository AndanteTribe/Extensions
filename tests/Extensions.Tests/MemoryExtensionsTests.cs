namespace Extensions.Tests;

public class MemoryExtensionsTests
{
    [Fact]
    public void GetEnumerator_Memory_EnumeratesAllElements()
    {
        Memory<int> memory = new[] { 1, 2, 3 };
        var result = new List<int>();
        foreach (var item in memory)
            result.Add(item);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void GetEnumerator_ReadOnlyMemory_EnumeratesAllElements()
    {
        ReadOnlyMemory<int> memory = new[] { 1, 2, 3 };
        var result = new List<int>();
        foreach (var item in memory)
            result.Add(item);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    // MoveNext has two false-return paths:
    //   Path A: ++_index < _memory.Length evaluates to false  (normal exhaustion)
    //   Path B: _index < _memory.Length short-circuits to false (called after exhaustion)
    // Both paths are exercised by calling MoveNext twice on an empty memory:
    //   Call 1: _index=-1 → -1 < 0 = true, ++(-1)=0 < 0 = false  → Path A
    //   Call 2: _index= 0 →  0 < 0 = false (short-circuit)        → Path B
    [Fact]
    public void GetEnumerator_Memory_Empty_BothFalsePathsCovered()
    {
        Memory<int> memory = new int[0];
        var e = memory.GetEnumerator();
        Assert.False(e.MoveNext()); // Path A: ++_index < _memory.Length = false
        Assert.False(e.MoveNext()); // Path B: _index < _memory.Length = false (short-circuit)
    }

    [Fact]
    public void GetEnumerator_ReadOnlyMemory_Empty_BothFalsePathsCovered()
    {
        ReadOnlyMemory<int> memory = new int[0];
        var e = memory.GetEnumerator();
        Assert.False(e.MoveNext()); // Path A
        Assert.False(e.MoveNext()); // Path B
    }
}
