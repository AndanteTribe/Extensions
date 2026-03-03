using System.Buffers;
using System.Runtime.CompilerServices;

namespace Extensions;

/// <summary>
/// Additional extension methods for <see cref="System.Collections.Generic"/>, <see cref="System.Linq"/>, and <see cref="System.Buffers.ArrayPool{T}"/>.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Converts a <see cref="List{T}"/> to a <see cref="Span{T}"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Example
    /// {
    ///    public static void Main()
    ///    {
    ///        List<int> numbers = new List<int> { 10, 20, 30, 40, 50 };
    ///
    ///        // Convert List<int> to Span<int>
    ///        Span<int> numbersSpan = numbers.AsSpan();
    ///
    ///        // Loop through the span (faster than using List<T> directly)
    ///        foreach (var item in numbersSpan)
    ///        {
    ///             Console.WriteLine(item);
    ///        }
    ///    }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="list">The List to convert.</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>A <see cref="Span{T}"/> containing the list elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this List<T> list) =>
        System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);

    /// <summary>
    /// Grows a borrowed array from <see cref="ArrayPool{T}"/> if needed.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Buffers;
    ///
    /// public class Example
    /// {
    ///    public static void Main()
    ///    {
    ///        var pool = ArrayPool<int>.Shared;
    ///        // Extend the existing array if it becomes too small
    ///        int[] buffer = pool.Rent(3);
    ///        try
    ///        {
    ///            // If a minimum of 10 elements is needed
    ///            pool.Grow(ref buffer, 10);
    ///            Console.WriteLine(buffer.Length); // => >= 10
    ///        }
    ///        finally
    ///        {
    ///            // Always return the buffer
    ///            pool.Return(buffer);
    ///        }
    ///    }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance.</param>
    /// <param name="array">The array reference to grow.</param>
    /// <param name="minimumLength">The minimum required length.</param>
    /// <typeparam name="T">The element type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Grow<T>(this ArrayPool<T> pool, ref T[] array, int minimumLength)
    {
        if (array.Length < minimumLength)
        {
            var newArray = pool.Rent(minimumLength);
            if (array.Length > 0)
            {
                array.AsSpan().CopyTo(newArray);
                pool.Return(array, true);
            }
            array = newArray;
        }
    }

    /// <summary>
    /// Gets a handle for returning an array borrowed from <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <remarks>
    /// Typically used in conjunction with a <see langword="using"/> statement.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Buffers;
    ///
    /// public class Example
    /// {
    ///    public static void Main()
    ///    {
    ///        // Use ArrayPool<int>.Shared to rent an array
    ///        using (var handle = ArrayPool<int>.Shared.Rent(10, out int[] array))
    ///        {
    ///            // Create a span with only the needed elements
    ///            var span = array.AsSpan(0, 10);
    ///            for (int i = 0; i < span.Length; i++)
    ///            {
    ///                span[i] = i;
    ///            }
    ///            Console.WriteLine(string.Join(",", span.ToArray()));
    ///        } // Dispose is called when exiting the scope, returning the array
    ///    }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance.</param>
    /// <param name="minimumLength">The minimum required length.</param>
    /// <param name="array">The rented array (output parameter).</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>A <see cref="Handle{T}"/> that returns the array when disposed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Handle<T> Rent<T>(this ArrayPool<T> pool, int minimumLength, out T[] array) =>
        new(pool, array = pool.Rent(minimumLength));

    /// <summary>
    /// A handle for returning an array borrowed from <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public readonly struct Handle<T> : IDisposable
    {
        private readonly ArrayPool<T> _pool;
        private readonly T[] _array;

        internal Handle(ArrayPool<T> pool, T[] array)
        {
            _pool = pool;
            _array = array;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() => _pool.Return(_array);
    }
}