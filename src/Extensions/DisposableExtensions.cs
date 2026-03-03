using System.Runtime.CompilerServices;

namespace Extensions;

/// <summary>
/// Extension methods for <see cref="IDisposable"/>.
/// </summary>
public static class DisposableExtensions
{
    /// <summary>
    /// Adds an <see cref="IDisposable"/> to a specified <see cref="IDisposable"/> container.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using System;
    ///
    /// public class Example
    /// {
    ///     private IDisposable _resource;
    ///
    ///     public void Initialize()
    ///     {
    ///         // Create a resource and add it to the container
    ///         var resource = CreateManagedResource();
    ///         resource.AddTo(ref _resource);
    ///     }
    ///
    ///     public void Cleanup()
    ///     {
    ///         _resource?.Dispose();
    ///     }
    ///
    ///     private IDisposable CreateManagedResource()
    ///     {
    ///         return new DisposableResource();
    ///     }
    /// }
    ///
    /// public class DisposableResource : IDisposable
    /// {
    ///     public void Dispose()
    ///     {
    ///         Console.WriteLine("Resource disposed");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="disposable">Any <see cref="IDisposable"/> implementation object.</param>
    /// <param name="disposableContainer">The <see cref="IDisposable"/> container to add to.</param>
    /// <typeparam name="T">The type of the added <see cref="IDisposable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddTo<T>(this T disposable, ref T disposableContainer) where T : IDisposable => disposableContainer = disposable;
}