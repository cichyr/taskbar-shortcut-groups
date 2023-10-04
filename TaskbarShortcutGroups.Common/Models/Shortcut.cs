using System.Diagnostics;
using System.Drawing;
using Icon = System.Drawing.Icon;
using System.Xml.Serialization;
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using TaskbarShortcutGroups.Common.Extensions;
using IPersistFile = Windows.Win32.System.Com.IPersistFile;

namespace TaskbarShortcutGroups.Common.Models;

public class Shortcut
{
    private IShellLinkW? shellLink;

    /// <summary>
    /// Creates new instance of <see cref="Shortcut" />.
    /// </summary>
    /// <param name="path"> The path to the shortcut that should be loaded. </param>
    /// <exception cref="ArgumentNullException"> When any parameter is <see langword="null" />. </exception>
    public Shortcut(string path)
    {
        Location = path ?? throw new ArgumentNullException(nameof(path));
        Name = Path.GetFileName(path);
    }

    /// <summary>
    /// Creates new instance of <see cref="Shortcut" />.
    /// </summary>
    public Shortcut()
    {
        Location = string.Empty;
    }

    private unsafe IShellLinkW ShellLink
    {
        get
        {
            if (shellLink != null)
                return shellLink;

            // ReSharper disable SuspiciousTypeConversion.Global - ComImport, nothing I can do
            shellLink = (IShellLinkW)new ShellLink();
            if (File.Exists(Location))
                fixed(char* pszFileName = Location)
                    ((IPersistFile)shellLink).Load(pszFileName, STGM.STGM_READ);
            // ReSharper restore SuspiciousTypeConversion.Global
            return shellLink;
        }
    }

    /// <summary>
    /// Gets or sets the name of the shortcut.
    /// </summary>
    [XmlAttribute(nameof(Name))]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the shortcut.
    /// </summary>
    [XmlAttribute(nameof(Location))]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets path to the executable.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    [XmlIgnore]
    public unsafe string ExecutablePath
    {
        get
        {
            fixed (char* pszFile = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink.GetPath(pszFile, (int)PInvoke.MAX_PATH, null, (uint)SLGP_FLAGS.SLGP_RAWPATH);
                return new string(pszFile);
            }
        }
        set
        {
            if (value.Length > PInvoke.MAX_PATH)
                throw new ArgumentException("The length of the new path should not exceed 260.");
            ShellLink.SetPath(value);
        }
    }

    /// <summary>
    /// Gets or sets the window style of the executed shortcut.
    /// </summary>
    /// <exception cref="ArgumentException"> When provided with unsupported <see cref="ProcessWindowStyle" /> value. </exception>
    [XmlIgnore]
    public ProcessWindowStyle WindowStyle
    {
        get
        {
            ShellLink.GetShowCmd(out var cmdShow);
            return cmdShow switch
            {
                SHOW_WINDOW_CMD.SW_HIDE => ProcessWindowStyle.Hidden,
                SHOW_WINDOW_CMD.SW_NORMAL => ProcessWindowStyle.Normal,
                SHOW_WINDOW_CMD.SW_MINIMIZE => ProcessWindowStyle.Minimized,
                SHOW_WINDOW_CMD.SW_MAXIMIZE => ProcessWindowStyle.Maximized,
                _ => ProcessWindowStyle.Normal
            };
        }
        set
        {
            var cmdShow = value switch
            {
                ProcessWindowStyle.Hidden => SHOW_WINDOW_CMD.SW_HIDE,
                ProcessWindowStyle.Normal => SHOW_WINDOW_CMD.SW_NORMAL,
                ProcessWindowStyle.Minimized => SHOW_WINDOW_CMD.SW_MINIMIZE,
                ProcessWindowStyle.Maximized => SHOW_WINDOW_CMD.SW_MAXIMIZE,
                _ => throw new ArgumentException("Unsupported value")
            };

            ShellLink.SetShowCmd(cmdShow);
        }
    }

    /// <summary>
    /// Gets or sets the working directory.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    [XmlIgnore]
    public unsafe string WorkingDirectory
    {
        get
        {
            fixed (char* pszDir = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink.GetWorkingDirectory(pszDir, 260);
                return new string(pszDir);
            }
        }
        set
        {
            if (value.Length > 260)
                throw new ArgumentException("The length of the new working directory path should not exceed 260.");
            ShellLink.SetWorkingDirectory(value);
        }
    }

    /// <summary>
    /// Gets or sets the startup arguments of the executable to which <see cref="ExecutablePath" /> is pointing to.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided description is too long. </exception>
    [XmlIgnore]
    public unsafe string Arguments
    {
        get
        {
            fixed (char* pszArgs = new char[1024])
            {
                ShellLink.GetArguments(pszArgs, 1024);
                return new string(pszArgs);
            }
        }
        set
        {
            if (value.Length > 1024)
                throw new ArgumentException("The length of the new arguments should not exceed 1024.");
            ShellLink.SetArguments(value);
        }
    }

    /// <summary>
    /// Gets or sets the icon location.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    [XmlIgnore]
    public unsafe IconLocation? IconLocation
    {
        get
        {
            string iconPath;
            int piIcon;
            fixed (char* pszIconPath = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink.GetIconLocation(pszIconPath, (int)PInvoke.MAX_PATH, out piIcon);
                iconPath = new string(pszIconPath);
            }

            return string.IsNullOrEmpty(iconPath)
                ? null
                : new IconLocation(iconPath, piIcon);
        }
        set
        {
            ExceptionExtensions.ThrowIfNull(value);
            if (value.FilePath.Length > 260)
                throw new ArgumentException("The length of the path to a new icon should not exceed 260.");
            ShellLink.SetIconLocation(value.FilePath, value.Address);
        }
    }

    public Bitmap IconBitmap
        => MemorySafeIcon ?? Icon.ExtractAssociatedIcon(ExecutablePath).ToBitmap();

    /// <summary>
    /// Gets the small icon for the shortcut.
    /// </summary>
    [XmlIgnore]
    public Icon? SmallIcon
    {
        get
        {
            if (IconLocation is null)
                return null;
            PInvoke.ExtractIconEx(IconLocation.FilePath, IconLocation.Address, out _, out var iconHandler, 1);
            var icon = Icon.FromHandle(iconHandler.DangerousGetHandle());
            return icon;
        }
    }

    public Bitmap? MemorySafeIcon
    {
        get
        {
            if (IconLocation is null)
                return null;
            using var iconHandle = PInvoke.ExtractIcon(IconLocation.FilePath, (uint)IconLocation.Address);
            var icon = Icon.FromHandle(iconHandle.DangerousGetHandle());
            return icon.ToBitmap();
        }
    }

    /// <summary>
    /// Gets the large icon for the shortcut.
    /// </summary>
    [XmlIgnore]
    public Bitmap? LargeIcon
    {
        get
        {
            if (IconLocation is null)
                return null;
            PInvoke.ExtractIconEx(IconLocation.FilePath, IconLocation.Address, out var iconHandler, out _, 1);
            var icon = Icon.FromHandle(iconHandler.DangerousGetHandle());
            return icon.ToBitmap();
        }
    }

    /// <summary>
    /// Saves the shortcut in provided path.
    /// </summary>
    /// <param name="path"> The path to the saving directory. </param>
    public void Save(string path)
        => ((IPersistFile)ShellLink).Save(Path.Combine(path, $"{Name}.lnk"), false);
}