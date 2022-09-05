using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskbarShortcutGroups.Models.ComImports;

/// <summary>
///     Exposes methods that create, modify, and resolve Shell links.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F9-0000-0000-C000-000000000046")]
public interface IShellLink
{
    /// <summary>
    ///     Gets the path and file name of the target of a Shell link object.
    /// </summary>
    /// <param name="pszFile">
    ///     The address of a buffer that receives the path and file name of the target of the Shell link
    ///     object.
    /// </param>
    /// <param name="cchMaxPath">
    ///     The size, in characters, of the buffer pointed to by the <paramref name="pszFile" />
    ///     parameter, including the terminating null character. The maximum path size that can be returned is 260.
    /// </param>
    /// <param name="pfd">
    ///     A pointer to a structure that receives information about the target of the Shell link object. If this
    ///     parameter is <see langword="null" />, then no additional information is returned.
    /// </param>
    /// <param name="fFlags">Flags that specify the type of path information to retrieve.</param>
    void GetPath([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath,
        out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);

    /// <summary>
    ///     Gets the list of item identifiers for the target of a Shell link object.
    /// </summary>
    /// <param name="ppidl">When this method returns, contains the address of an item identifier list (PIDL).</param>
    void GetIDList(out IntPtr ppidl);

    /// <summary>
    ///     Sets the pointer to an item identifier list (PIDL) for a Shell link object.
    /// </summary>
    /// <param name="pidl">The object's fully qualified item identifier list.</param>
    void SetIDList(IntPtr pidl);

    /// <summary>
    ///     Gets the description string for a Shell link object.
    /// </summary>
    /// <param name="pszName">A pointer to the buffer that receives the description string.</param>
    /// <param name="cchMaxName">
    ///     The maximum number of characters to copy to the buffer pointed to by the
    ///     <paramref name="pszName" />.
    /// </param>
    void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

    /// <summary>
    ///     Sets the description for a Shell link object. The description can be any application-defined string.
    /// </summary>
    /// <param name="pszName">A pointer to a buffer containing the new description string.</param>
    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    /// <summary>
    ///     Gets the name of the working directory for a Shell link object.
    /// </summary>
    /// <param name="pszDir">The address of a buffer that receives the name of the working directory.</param>
    /// <param name="cchMaxPath">
    ///     The maximum number of characters to copy to the buffer pointed to by the
    ///     <paramref name="pszDir" />. The name of the working directory is truncated if it is longer than the maximum
    ///     specified by this parameter.
    /// </param>
    void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

    /// <summary>
    ///     Sets the name of the working directory for a Shell link object.
    /// </summary>
    /// <param name="pszDir">The address of a buffer that contains the name of the new working directory.</param>
    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

    /// <summary>
    ///     Gets the command-line arguments associated with a Shell link object.
    /// </summary>
    /// <param name="pszArgs">
    ///     A pointer to the buffer that, when this method returns successfully, receives the command-line
    ///     arguments.
    /// </param>
    /// <param name="cchMaxPath">
    ///     The maximum number of characters that can be copied to the buffer supplied by the
    ///     <paramref name="pszArgs" />.
    /// </param>
    void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

    /// <summary>
    ///     Sets the command-line arguments for a Shell link object.
    /// </summary>
    /// <param name="pszArgs">A pointer to a buffer that contains the new command-line arguments.</param>
    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

    /// <summary>
    ///     Gets the keyboard shortcut (hot key) for a Shell link object.
    /// </summary>
    /// <param name="pwHotkey">
    ///     The address of the keyboard shortcut. The virtual key code is in the low-order byte, and the
    ///     modifier flags are in the high-order byte.
    /// </param>
    void GetHotkey(out short pwHotkey);

    /// <summary>
    ///     Sets a keyboard shortcut (hot key) for a Shell link object.
    /// </summary>
    /// <param name="wHotkey">
    ///     The new keyboard shortcut. The virtual key code is in the low-order byte, and the modifier flags
    ///     are in the high-order byte.
    /// </param>
    void SetHotkey(short wHotkey);

    /// <summary>
    ///     Gets the show command for a Shell link object.
    /// </summary>
    /// <param name="piShowCmd">A pointer to the command.</param>
    void GetShowCmd(out int piShowCmd);

    /// <summary>
    ///     Sets the show command for a Shell link object. The show command sets the initial show state of the window.
    /// </summary>
    /// <param name="iShowCmd">Command.</param>
    void SetShowCmd(int iShowCmd);

    /// <summary>
    ///     Gets the location (path and index) of the icon for a Shell link object.
    /// </summary>
    /// <param name="pszIconPath">The address of a buffer that receives the path of the file containing the icon.</param>
    /// <param name="cchIconPath">
    ///     The maximum number of characters to copy to the buffer pointed to by the pszIconPath
    ///     parameter.
    /// </param>
    /// <param name="piIcon">The address of a value that receives the index of the icon.</param>
    void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath,
        out int piIcon);

    /// <summary>
    ///     Sets the location (path and index) of the icon for a Shell link object.
    /// </summary>
    /// <param name="pszIconPath">The address of a buffer to contain the path of the file containing the icon.</param>
    /// <param name="iIcon">The index of the icon.</param>
    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

    /// <summary>
    ///     Sets the relative path to the Shell link object.
    /// </summary>
    /// <param name="pszPathRel">
    ///     The address of a buffer that contains the fully-qualified path of the shortcut file, relative
    ///     to which the shortcut resolution should be performed. It should be a file name, not a folder name.
    /// </param>
    /// <param name="dwReserved">Reserved. Set this parameter to zero.</param>
    void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

    /// <summary>
    ///     Attempts to find the target of a Shell link, even if it has been moved or renamed.
    /// </summary>
    /// <param name="hwnd">
    ///     A handle to the window that the Shell will use as the parent for a dialog box. The Shell displays
    ///     the dialog box if it needs to prompt the user for more information while resolving a Shell link.
    /// </param>
    /// <param name="fFlags">Action flags.</param>
    void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);

    /// <summary>
    ///     Sets the path and file name for the target of a Shell link object.
    /// </summary>
    /// <param name="pszFile">The address of a buffer that contains the new path.</param>
    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}