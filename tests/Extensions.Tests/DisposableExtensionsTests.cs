namespace Extensions.Tests;

public class DisposableExtensionsTests
{
    private sealed class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }

    [Fact]
    public void AddTo_SetsDisposableContainer()
    {
        TestDisposable container = null!;
        var resource = new TestDisposable();
        resource.AddTo(ref container);
        Assert.Same(resource, container);
    }
}
