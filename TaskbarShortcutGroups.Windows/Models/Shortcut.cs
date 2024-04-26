using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Windows.Models;

public unsafe class Shortcut : IShortcut
{
    private Bitmap? iconBitmap;
    private IShellLinkW* shellLink = null;

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
    /// Gets the pointer to the shell link.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native COM/Shell/Win32/PInvoke naming")]
    private IShellLinkW* ShellLink
    {
        get
        {
            if (shellLink != null)
                return shellLink;

            var hres = PInvoke.CoInitialize();
            if (hres.Failed)
                throw new InvalidOperationException($"Failed to initialize COM context with failure code '{hres.Value}'");

            hres = PInvoke.CoCreateInstance(typeof(ShellLink).GUID, default, CLSCTX.CLSCTX_INPROC_SERVER, out shellLink);
            if (hres.Failed)
                throw new InvalidOperationException($"Failed to create shell link with failure code '{hres.Value}'");

            if (File.Exists(Location))
                fixed (char* pszFileName = Location)
                {
                    var persistFile = GetPersistFile();
                    persistFile->Load(pszFileName, STGM.STGM_READ);
                    persistFile->Release();
                }

            return shellLink;
        }
    }

    /// <inheritdoc />
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Location { get; set; }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native win32 shell naming")]
    public string ExecutablePath
    {
        get
        {
            fixed (char* pszFile = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink->GetPath(pszFile, (int)PInvoke.MAX_PATH, null, (uint)SLGP_FLAGS.SLGP_RAWPATH);
                return new string(pszFile);
            }
        }
        set
        {
            if (value.Length > PInvoke.MAX_PATH)
                throw new ArgumentException("The length of the new path should not exceed 260.");
            ShellLink->SetPath(value);
        }
    }

    /// <inheritdoc />
    public ProcessWindowStyle WindowStyle
    {
        get
        {
            ShellLink->GetShowCmd(out var cmdShow);
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

            ShellLink->SetShowCmd(cmdShow);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native win32 shell naming")]
    public string WorkingDirectory
    {
        get
        {
            fixed (char* pszDir = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink->GetWorkingDirectory(pszDir, 260);
                return new string(pszDir);
            }
        }
        set
        {
            if (value.Length > 260)
                throw new ArgumentException("The length of the new working directory path should not exceed 260.");
            ShellLink->SetWorkingDirectory(value);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native win32 shell naming")]
    public string Arguments
    {
        get
        {
            fixed (char* pszArgs = new char[1024])
            {
                ShellLink->GetArguments(pszArgs, 1024);
                return new string(pszArgs);
            }
        }
        set
        {
            if (value.Length > 1024)
                throw new ArgumentException("The length of the new arguments should not exceed 1024.");
            ShellLink->SetArguments(value);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native win32 shell naming")]
    public IconLocation? IconLocation
    {
        get
        {
            string iconPath;
            int piIcon;
            fixed (char* pszIconPath = new char[(int)PInvoke.MAX_PATH])
            {
                ShellLink->GetIconLocation(pszIconPath, (int)PInvoke.MAX_PATH, out piIcon);
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
            ShellLink->SetIconLocation(value.FilePath, value.Address);
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
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native win32 shell naming")]
    public void Save(string path)
    {
        var persistFile = GetPersistFile();
        persistFile->Save(Path.Combine(path, $"{Name}.lnk"), true);
        persistFile->Release();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        PInvoke.CoUninitialize();
        iconBitmap?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Queries the current <see cref="ShellLink" /> for <see cref="IPersistFile" /> interface and returns it.
    /// </summary>
    /// <remarks> Don't forget to call <see cref="IUnknown.Release" /> when you're done using it. </remarks>
    /// <exception cref="InvalidOperationException"> When querying <see cref="ShellLink" /> fails. </exception>
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Used native COM/Shell/Win32/PInvoke naming")]
    private IPersistFile* GetPersistFile()
    {
        var hres = ShellLink->QueryInterface(IPersistFile.IID_Guid, out var ppvObject);
        if (!hres.Succeeded)
            throw new InvalidOperationException($"Failed to convert shell link to persistence file with failure code '{hres.Value}'");
        return (IPersistFile*)ppvObject;
    }
}