using System.Runtime.InteropServices;

namespace TaskbarShortcutGroups.Common.Models.ComImports;

/// <summary>
///     Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC).
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct FILETIME
{
    /// <summary>
    ///     The low-order part of the file time.
    /// </summary>
    public uint dwLowDateTime;

    /// <summary>
    ///     The high-order part of the file time.
    /// </summary>
    public uint dwHighDateTime;
}