using System.Runtime.InteropServices;

namespace TaskbarShortcutGroups.Models.ComImports;

/// <summary>
///     Contains information about the file that is found by the <c>FindFirstFile</c>, <c>FindFirstFileEx</c>, or
///     <c>FindNextFile</c> function.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
// ReSharper disable once InconsistentNaming - Preserving minwinbase.h naming
public struct WIN32_FIND_DATAW
{
    /// <summary>
    ///     The file attributes of a file.
    /// </summary>
    /// <remarks>
    ///     Possible values: https://docs.microsoft.com/en-us/windows/win32/fileio/file-attribute-constants
    /// </remarks>
    public uint dwFileAttributes;

    /// <summary>
    ///     Specifies when a file or directory was created.
    /// </summary>
    public uint ftCreationTime;

    /// <summary>
    ///     For a file, the structure specifies when the file was last read from, written to, or for executable files, run.
    ///     For a directory, the structure specifies when the directory is created. If the underlying file system does not
    ///     support last access time, this member is zero.
    /// </summary>
    /// <remarks>
    ///     On the FAT file system, the specified date for both files and directories is correct, but the time of day is
    ///     always set to midnight.
    /// </remarks>
    public long ftLastAccessTime;

    /// <summary>
    ///     For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example,
    ///     when <c>WriteFile</c> or <c>SetEndOfFile</c> are used. The date and time are not updated when file attributes or
    ///     security descriptors are changed. For a directory, the structure specifies when the directory is created. If the
    ///     underlying file system does not support last write time, this member is zero.
    /// </summary>
    public long ftLastWriteTime;

    /// <summary>
    ///     The high-order <see cref="uint" /> value of the file size, in bytes. This value is zero unless the file size is
    ///     greater than
    ///     <see cref="uint.MaxValue" />.
    /// </summary>
    /// <remarks>
    ///     The size of the file is equal to: (<see cref="WIN32_FIND_DATAW.nFileSizeHigh" /><c> * (</c>
    ///     <see cref="uint.MaxValue" /><c>+1)) + </c><see cref="WIN32_FIND_DATAW.nFileSizeLow" />.
    /// </remarks>
    public uint nFileSizeHigh;

    /// <summary>
    ///     The low-order <see cref="uint" /> value of the file size, in bytes.
    /// </summary>
    /// <remarks>
    ///     The size of the file is equal to: (<see cref="WIN32_FIND_DATAW.nFileSizeHigh" /><c> * (</c>
    ///     <see cref="uint.MaxValue" /><c>+1)) + </c><see cref="WIN32_FIND_DATAW.nFileSizeLow" />.
    /// </remarks>
    public uint nFileSizeLow;

    /// <summary>
    ///     If the <see cref="WIN32_FIND_DATAW.dwFileAttributes" /> member includes the FILE_ATTRIBUTE_REPARSE_POINT
    ///     attribute, this member specifies the reparse point tag. Otherwise, this value is undefined and should not be used.
    /// </summary>
    public uint dwReserved0;

    /// <summary>
    ///     Reserved for future use.
    /// </summary>
    public uint dwReserved1;

    /// <summary>
    ///     The name of the file.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string cFileName;

    /// <summary>
    ///     An alternative name for the file.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    public string cAlternateFileName;
}