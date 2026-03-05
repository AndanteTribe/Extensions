using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Extensions;

/// <summary>
/// Extension methods for <see cref="ValueTuple"/>.
/// </summary>
public static class ValueTupleExtensions
{
    /// <summary>
    /// Converts a 2-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         // Iterate through the span
    ///         var span = ("Hello", "World").AsSpan();
    ///         foreach (var item in span)
    ///         {
    ///             Console.WriteLine(item);
    ///         }
    ///         // Output:
    ///         // Hello
    ///         // World
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 2-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 2 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 2);

    /// <summary>
    /// Converts a 3-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 3-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 3 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 3);

    /// <summary>
    /// Converts a 4-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 4-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 4 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 4);

    /// <summary>
    /// Converts a 5-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 5-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 5 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 5);

    /// <summary>
    /// Converts a 6-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 6-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 6 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 6);

    /// <summary>
    /// Converts a 7-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 7-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 7 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(ref this (T, T, T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 7);

    /// <summary>
    /// Returns an enumerator for a 2-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// foreach (var item in (1, 2))
    /// {
    ///     Console.WriteLine(item);
    /// }
    /// // Output:
    /// // 1
    /// // 2
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 2-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T) tuple)
    {
        var span = tuple.AsSpan();
        var result = new Enumerator<T>(span);
        return result;
    }

    /// <summary>
    /// Returns an enumerator for a 3-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 3-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T, T) tuple) => new(tuple.AsSpan());

    /// <summary>
    /// Returns an enumerator for a 4-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 4-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T, T, T) tuple) => new(tuple.AsSpan());

    /// <summary>
    /// Returns an enumerator for a 5-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 5-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T, T, T, T) tuple) => new(tuple.AsSpan());

    /// <summary>
    /// Returns an enumerator for a 6-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 6-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T, T, T, T, T) tuple) => new(tuple.AsSpan());

    /// <summary>
    /// Returns an enumerator for a 7-element ValueTuple, enabling foreach iteration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 7-element ValueTuple to enumerate.</param>
    /// <returns>An enumerator for the tuple elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this (T, T, T, T, T, T, T) tuple) => new(tuple.AsSpan());

    /// <summary>
    /// A custom enumerator that enables foreach iteration over ValueTuple elements.
    /// </summary>
    /// <remarks>
    /// This enumerator rents an array from <see cref="ArrayPool{T}"/> to safely store tuple elements,
    /// ensuring they remain valid throughout the enumeration even when the original tuple goes out of scope.
    /// The array is automatically returned to the pool when the enumerator is disposed.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Using foreach (automatic disposal)
    /// foreach (var num in (10, 20, 30))
    /// {
    ///     Console.WriteLine(num);
    /// }
    ///
    /// // Manual enumeration (requires disposal)
    /// using var enumerator = (1, 2, 3).GetEnumerator();
    /// while (enumerator.MoveNext())
    /// {
    ///     Console.WriteLine(enumerator.Current);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of elements being enumerated.</typeparam>
    public struct Enumerator<T> : IDisposable
    {
        private readonly T[] _values;
        private readonly int _end;
        private int _current;

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>
        public T Current => _values[_current];

        internal Enumerator(ReadOnlySpan<T> tupleSpan)
        {
            _values = ArrayPool<T>.Shared.Rent(tupleSpan.Length);
            tupleSpan.CopyTo(_values);
            _end = tupleSpan.Length;
            _current = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        /// <see cref="System.Collections.IEnumerator.MoveNext"/>
        public bool MoveNext() => _current < _end && ++_current < _end;

        /// <summary>
        /// Returns the rented array back to the <see cref="ArrayPool{T}"/>.
        /// </summary>
        /// <inheritdoc />
        void IDisposable.Dispose() => ArrayPool<T>.Shared.Return(_values);
    }
}