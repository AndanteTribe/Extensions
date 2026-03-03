using System.Runtime.CompilerServices;

namespace Extensions;

/// <summary>
/// Extension methods for enumerations.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Determines whether the specified bit flag is set in the value.
    /// </summary>
    /// <param name="value">The enum value to check.</param>
    /// <param name="flag">The flag to check for.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [System.Flags]
    /// public enum FileAccess
    /// {
    ///     None = 0,
    ///     Read = 1 << 0,      // 0001
    ///     Write = 1 << 1,     // 0010
    ///     Execute = 1 << 2,   // 0100
    ///     ReadWrite = Read | Write, // 0011
    /// }
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         // Set current permissions
    ///         const FileAccess currentPermissions = FileAccess.ReadWrite;
    ///
    ///         // 1. Check for a single flag
    ///         // Does it have Read permission? (true)
    ///         var hasRead = currentPermissions.HasBitFlags(FileAccess.Read);
    ///         Console.WriteLine($"Has Read permission: {hasRead}");
    ///
    ///         // Does it have Execute permission? (false)
    ///         var hasExecute = currentPermissions.HasBitFlags(FileAccess.Execute);
    ///         Console.WriteLine($"Has Execute permission: {hasExecute}");
    ///
    ///         // 2. Check multiple flags together
    ///         // Does it have both Read and Write permission? (true)
    ///         var hasReadWrite = currentPermissions.HasBitFlags(FileAccess.ReadWrite);
    ///         Console.WriteLine($"Has Read and Write permission: {hasReadWrite}");
    ///
    ///         // 3. Check for flags it doesn't have
    ///         const FileAccess fullAccess = FileAccess.Read | FileAccess.Write | FileAccess.Execute;
    ///
    ///         // Does it have all permissions? (false)
    ///         var hasFullAccess = currentPermissions.HasBitFlags(fullAccess);
    ///         Console.WriteLine($"Has all permissions: {hasFullAccess}");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <returns>True if all bits in the flag are set in the value; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">Thrown when the underlying type of the enum is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasBitFlags<T>(this T value, T flag) where T : struct, Enum
    {
        var (v, f) = Unsafe.SizeOf<T>() switch
        {
            1 => (Unsafe.As<T, byte>(ref value), Unsafe.As<T, byte>(ref flag)),
            2 => (Unsafe.As<T, ushort>(ref value), Unsafe.As<T, ushort>(ref flag)),
            4 => (Unsafe.As<T, uint>(ref value), Unsafe.As<T, uint>(ref flag)),
            8 => (Unsafe.As<T, ulong>(ref value), Unsafe.As<T, ulong>(ref flag)),
            _ => throw new NotSupportedException("Unsupported enum underlying type size.")
        };
        return (v & f) == f;
    }

    /// <summary>
    /// Determines whether the value has exactly one bit flag set.
    /// </summary>
    /// <param name="value">The enum value to check.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [System.Flags]
    /// public enum FileAccess
    /// {
    ///     None = 0,
    ///     Read = 1 << 0,      // 0001
    ///     Write = 1 << 1,     // 0010
    ///     Execute = 1 << 2,   // 0100
    ///     ReadWrite = Read | Write, // 0011
    ///     All = Read | Write | Execute
    /// }
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         // Only Read has a single flag
    ///         // Is a single flag: (true)
    ///         Console.WriteLine($"Is single flag: {FileAccess.Read.ConstructFlags()}");
    ///
    ///         // ReadWrite has two flags, not a single flag
    ///         // Is a single flag: (false)
    ///         Console.WriteLine($"Is single flag: {FileAccess.ReadWrite.ConstructFlags()}");
    ///
    ///         // All has three flags, not a single flag
    ///         // Is a single flag: (false)
    ///         Console.WriteLine($"Is single flag: {FileAccess.All.ConstructFlags()}");
    ///
    ///         // None has no flags, not a single flag
    ///         // Is a single flag: (false)
    ///         Console.WriteLine($"Is single flag: {FileAccess.None.ConstructFlags()}");
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <returns>True if the value has exactly one bit flag set; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">Thrown when the underlying type of the enum is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ConstructFlags<T>(this T value) where T : struct, Enum
    {
        var v = Unsafe.SizeOf<T>() switch
        {
            1 => Unsafe.As<T, byte>(ref value),
            2 => Unsafe.As<T, ushort>(ref value),
            4 => Unsafe.As<T, uint>(ref value),
            8 => Unsafe.As<T, ulong>(ref value),
            _ => throw new NotSupportedException("Unsupported enum underlying type size.")
        };
        return v != 0 && (v & (v - 1)) == 0;
    }

    /// <summary>
    /// Aggregates multiple bit flags from a span into a single enum value.
    /// </summary>
    /// <param name="flags">A span of enum flags to aggregate.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A single enum value representing the bitwise OR of all flags.</returns>
    /// <exception cref="NotSupportedException">Thrown when the underlying type of the enum is not supported.</exception>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [System.Flags]
    /// public enum FileAccess
    /// {
    ///     None = 0,
    ///     Read = 1 << 0,      // 0001
    ///     Write = 1 << 1,     // 0010
    ///     Execute = 1 << 2,   // 0100
    ///     ReadWrite = Read | Write,
    ///     All = Read | Write | Execute
    /// }
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         // Create a span from an array and aggregate
    ///         var flags = new[] { FileAccess.Read, FileAccess.Write };
    ///         var aggregated = flags.AsSpan().AggregateFlags();
    ///         // Output: ReadWrite
    ///         Console.WriteLine(aggregated);
    ///
    ///         // Single element
    ///         var single = new[] { FileAccess.Execute };
    ///         var aggregatedSingle = single.AsSpan().AggregateFlags();
    ///         // Output: Execute
    ///         Console.WriteLine(aggregatedSingle);
    ///
    ///         // Empty array -> default (= None)
    ///         var empty = Array.Empty<FileAccess>();
    ///         var aggregatedEmpty = empty.AsSpan().AggregateFlags();
    ///         // Output: None
    ///         Console.WriteLine(aggregatedEmpty);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AggregateFlags<T>(this Span<T> flags) where T : struct, Enum => AggregateFlags((ReadOnlySpan<T>)flags);

    /// <summary>
    /// Aggregates multiple bit flags from a read-only span into a single enum value.
    /// </summary>
    /// <param name="flags">A read-only span of enum flags to aggregate.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A single enum value representing the bitwise OR of all flags.</returns>
    /// <exception cref="NotSupportedException">Thrown when the underlying type of the enum is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AggregateFlags<T>(this ReadOnlySpan<T> flags) where T : struct, Enum
    {
        switch (Unsafe.SizeOf<T>())
        {
            case 1:
                var r1 = default(byte);
                var e1 = flags.GetEnumerator();
                while (e1.MoveNext())
                {
                    var flag = e1.Current;
                    var f = Unsafe.As<T, byte>(ref flag);
                    r1 = (byte)(r1 | f);
                }
                return Unsafe.As<byte, T>(ref r1);
            case 2:
                var r2 = default(short);
                var e2 = flags.GetEnumerator();
                while (e2.MoveNext())
                {
                    var flag = e2.Current;
                    var f = Unsafe.As<T, short>(ref flag);
                    r2 = (short)(r2 | f);
                }
                return Unsafe.As<short, T>(ref r2);
            case 4:
                var r4 = 0;
                var e4 = flags.GetEnumerator();
                while (e4.MoveNext())
                {
                    var flag = e4.Current;
                    var f = Unsafe.As<T, int>(ref flag);
                    r4 |= f;
                }
                return Unsafe.As<int, T>(ref r4);
            case 8:
                var r8 = 0L;
                var e8 = flags.GetEnumerator();
                while (e8.MoveNext())
                {
                    var flag = e8.Current;
                    var f = Unsafe.As<T, long>(ref flag);
                    r8 |= f;
                }
                return Unsafe.As<long, T>(ref r8);
            default:
                throw new NotSupportedException("Unsupported enum underlying type size.");
        }
    }

    /// <summary>
    /// Enumerates all individual bit flags set in the enum value.
    /// </summary>
    /// <param name="value">The enum value to enumerate.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [System.Flags]
    /// public enum FileAccess
    /// {
    ///     None = 0,
    ///     Read = 1 << 0,      // 0001
    ///     Write = 1 << 1,     // 0010
    ///     Execute = 1 << 2,   // 0100
    ///     ReadWrite = Read | Write, // 0011
    ///     All = Read | Write | Execute
    /// }
    ///
    /// public class Example
    /// {
    ///     public static void Main()
    ///     {
    ///         FileAccess currentPermissions = FileAccess.Read | FileAccess.Write;
    ///
    ///         // Enumerate individual flags
    ///         foreach (var flag in currentPermissions)
    ///         {
    ///             // Output:
    ///             // Read
    ///             // Write
    ///             Console.WriteLine(flag);
    ///         }
    ///
    ///         currentPermissions = FileAccess.None | FileAccess.All;
    ///
    ///         foreach (var flag in currentPermissions)
    ///         {
    ///             // Output: None is not printed
    ///             // Read
    ///             // Write
    ///             // Execute
    ///             Console.WriteLine(flag);
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <returns>An <see cref="Enumerator{T}"/> that enumerates all set bit flags.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Enumerator<T> GetEnumerator<T>(this T value) where T : struct, Enum => new(value);

    /// <summary>
    /// Enumerator for bit flags in an enum value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public struct Enumerator<T> where T : struct, Enum
    {
        private int _value;

        /// <see cref="System.Collections.Generic.IEnumerator{T}.Current"/>
        public T Current { get; private set; }

        internal Enumerator(T value)
        {
            _value = Unsafe.As<T, int>(ref value);
            Current = default;
        }

        /// <see cref="System.Collections.IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            if (_value == 0)
            {
                return false;
            }

            var f = _value & -_value; // get lowest flag
            Current = Unsafe.As<int, T>(ref f);
            _value &= ~f;
            return true;
        }
    }
}