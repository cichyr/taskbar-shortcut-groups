using System.Diagnostics;
using System.Drawing;
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Windows.Models;

public class Shortcut : IShortcut
{
    private Bitmap? iconBitmap;
    private IShellLinkW? shellLink;

    /// <summary>
    /// Creates a new instance of <see cref="Shortcut" />.
    /// </summary>
    /// <param name="path"> The path to the shortcut that should be loaded. </param>
    /// <exception cref="ArgumentNullException"> When any parameter is <see langword="null" />. </exception>
    public Shortcut(string path)
    {
        Location = path ?? throw new ArgumentNullException(nameof(path));
        Name = Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Shortcut" />.
    /// </summary>
    public Shortcut()
    {
        Location = string.Empty;
    }

    /// <summary>
    /// Gets the shell link.
    /// </summary>
    private unsafe IShellLinkW ShellLink
    {
        get
        {
            if (shellLink != null)
                return shellLink;

            shellLink = (IShellLinkW)new ShellLink();
            if (File.Exists(Location))
                fixed (char* pszFileName = Location)
                {
                    ((IPersistFile)shellLink).Load(pszFileName, STGM.STGM_READ);
                }

            return shellLink;
        }
    }

    /// <inheritdoc />
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Location { get; set; }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Bitmap? IconBitmap
    {
        get
        {
            if (iconBitmap is not null)
                return iconBitmap;

            if (IconLocation is null)
            {
                iconBitmap = Icon.ExtractAssociatedIcon(ExecutablePath)?.ToBitmap() ?? null;
                return iconBitmap;
            }

            using var iconHandle = PInvoke.ExtractIcon(IconLocation.FilePath, (uint)IconLocation.Address);
            var icon = Icon.FromHandle(iconHandle.DangerousGetHandle());
            iconBitmap = icon.ToBitmap();
            return iconBitmap;
        }
    }

    /// <inheritdoc />
    public int Order { get; set; }

    /// <summary>
    /// Saves the shortcut in provided path.
    /// </summary>
    /// <param name="path"> The path to the saving directory. </param>
    public void Save(string path)
        => (ShellLink as IPersistFile).Save(Path.Combine(path, $"{Name}.lnk"), false);

    /// <inheritdoc />
    public void Dispose()
    {
        iconBitmap?.Dispose();
        GC.SuppressFinalize(this);
    }
}