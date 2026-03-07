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
        switch (Unsafe.SizeOf<T>())
        {
            case 1:
                var v = Unsafe.As<T, byte>(ref value);
                var f = Unsafe.As<T, byte>(ref flag);
                return (v & f) == f;
            case 2:
                var v2 = Unsafe.As<T, ushort>(ref value);
                var f2 = Unsafe.As<T, ushort>(ref flag);
                return (v2 & f2) == f2;
            case 4:
                var v4 = Unsafe.As<T, uint>(ref value);
                var f4 = Unsafe.As<T, uint>(ref flag);
                return (v4 & f4) == f4;
            case 8:
                var v8 = Unsafe.As<T, ulong>(ref value);
                var f8 = Unsafe.As<T, ulong>(ref flag);
                return (v8 & f8) == f8;
            default:
                throw new NotSupportedException("Unsupported enum underlying type size.");
        }
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
        switch (Unsafe.SizeOf<T>())
        {
            case 1:
                var v = Unsafe.As<T, byte>(ref value);
                return v != 0 && (v & (v - 1)) == 0;
            case 2:
                var v2 = Unsafe.As<T, ushort>(ref value);
                return v2 != 0 && (v2 & (v2 - 1)) == 0;
            case 4:
                var v4 = Unsafe.As<T, uint>(ref value);
                return v4 != 0 && (v4 & (v4 - 1)) == 0;
            case 8:
                var v8 = Unsafe.As<T, ulong>(ref value);
                return v8 != 0 && (v8 & (v8 - 1)) == 0;
            default:
                throw new NotSupportedException("Unsupported enum underlying type size.");
        }
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
    /// <remarks>
    /// This method internally reinterprets the enum value as a 32-bit integer using <see cref="System.Runtime.CompilerServices.Unsafe"/>.
    /// It should only be used with enums whose underlying type is <see langword="int"/> (the default).
    /// Enums with other underlying types (e.g., <see langword="byte"/>, <see langword="long"/>) are not supported and may produce incorrect results.
    /// </remarks>
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
    public static Enumerator<T> GetEnumerator<T>(this T value) where T : struct, Enum
    {
        if (Enum.GetUnderlyingType(typeof(T)) != typeof(int))
        {
            throw new NotSupportedException("Only enums with int underlying type are supported for enumeration.");
        }
        return new Enumerator<T>(value);
    }

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