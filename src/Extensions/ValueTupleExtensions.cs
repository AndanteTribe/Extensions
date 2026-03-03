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
    ///         foreach (var item in ("Hello", "World").AsSpan())
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
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 2);

    /// <summary>
    /// Converts a 3-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 3-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 3 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 3);

    /// <summary>
    /// Converts a 4-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 4-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 4 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 4);

    /// <summary>
    /// Converts a 5-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 5-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 5 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 5);

    /// <summary>
    /// Converts a 6-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 6-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 6 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 6);

    /// <summary>
    /// Converts a 7-element ValueTuple to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the tuple.</typeparam>
    /// <param name="tuple">The 7-element ValueTuple to convert.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> with 7 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this (T, T, T, T, T, T, T) tuple) =>
        MemoryMarshal.CreateReadOnlySpan(ref tuple.Item1, 7);
}