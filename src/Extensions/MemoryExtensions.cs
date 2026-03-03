using System.Collections;
using System.Runtime.CompilerServices;

namespace Extensions;

/// <summary>
/// Extension methods for <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
public static class MemoryExtensions
{
    /// <summary>
    /// Enables foreach iteration over a <see cref="Memory{T}"/>.
    /// </summary>
    /// <remarks>
    /// This method works the same way as <see cref="GetEnumerator{T}(in ReadOnlyMemory{T})"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using System;
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         const string text = "Hello, World!";
    ///         Memory<char> textMemory = text.AsMemory().Slice(7, 5); // 'W', 'o', 'r', 'l', 'd'
    ///
    ///         foreach (var c in textMemory)
    ///         {
    ///             Console.WriteLine(c);
    ///         }
    ///         // Output:
    ///         // W
    ///         // o
    ///         // r
    ///         // l
    ///         // d
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="memory">The target <see cref="Memory{T}"/>.</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>An <see cref="MemoryExtensions.Enumerator{T}"/> that enumerates the elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(in this Memory<T> memory) => new(memory);

    /// <summary>
    /// Enables foreach iteration over a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using System;
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         const string text = "Hello, World!";
    ///         ReadOnlyMemory<char> textMemory = text.AsMemory().Slice(7, 5); // 'W', 'o', 'r', 'l', 'd'
    ///
    ///         foreach (var c in textMemory)
    ///         {
    ///             Console.WriteLine(c);
    ///         }
    ///         // Output:
    ///         // W
    ///         // o
    ///         // r
    ///         // l
    ///         // d
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="memory">The target <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>An <see cref="MemoryExtensions.Enumerator{T}"/> that enumerates the elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(in this ReadOnlyMemory<T> memory) => new(memory);

    /// <summary>
    /// Enumerator for foreach iteration over <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public struct Enumerator<T>
    {
        private readonly ReadOnlyMemory<T> _memory;
        private int _index;

        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>
        public readonly T Current => _memory.Span[_index];

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator{T}"/> struct.
        /// </summary>
        /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to enumerate.</param>
        internal Enumerator(in ReadOnlyMemory<T> memory)
        {
            _memory = memory;
            _index = -1;
        }

        /// <see cref="System.Collections.IEnumerator.MoveNext"/>
        public bool MoveNext() => _index < _memory.Length && ++_index < _memory.Length;
    }
}