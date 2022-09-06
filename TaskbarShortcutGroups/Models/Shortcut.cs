using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TaskbarShortcutGroups.Models.ComImports;

namespace TaskbarShortcutGroups.Models;

public class Shortcut
{
    private readonly IShellLink shellLink;

    /// <summary>
    ///     Creates new instance of <see cref="Shortcut" />.
    /// </summary>
    /// <param name="path">The path to the shortcut that should be loaded.</param>
    /// <exception cref="ArgumentException">When any parameter is <see langword="null" />.</exception>
    public Shortcut(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        shellLink = (IShellLink) new ShellLink();
        if (Path.GetExtension(path) == ".lnk")
            ((IPersistFile) shellLink).Load(path, 0);
        else
            ExecutablePath = path;
        Name = Path.GetFileName(path);
    }

    /// <summary>
    ///     Creates new instance of <see cref="Shortcut" />.
    /// </summary>
    public Shortcut()
    {
        shellLink = (IShellLink) new ShellLink();
    }

    /// <summary>
    ///     Gets or sets the name of the shortcut.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets path to the executable.
    /// </summary>
    /// <exception cref="ArgumentException">If provided path is longer than allowed (260 characters).</exception>
    public string ExecutablePath
    {
        get
        {
            var path = new StringBuilder(260);
            shellLink.GetPath(path, path.Capacity, out _, 0);
            return path.ToString();
        }
        set
        {
            if (value.Length > 260) throw new ArgumentException("The length of the new path should not exceed 260.");
            shellLink.SetPath(value);
        }
    }

    /// <summary>
    ///     Gets or sets the description
    /// </summary>
    public string Description
    {
        get
        {
            var description = new StringBuilder(1024);
            shellLink.GetDescription(description, description.Capacity);
            return description.ToString();
        }
        set => shellLink.SetDescription(value);
    }

    /// <summary>
    ///     Gets or sets the working directory.
    /// </summary>
    /// <exception cref="ArgumentException">If provided path is longer than allowed (260 characters).</exception>
    public string WorkingDirectory
    {
        get
        {
            var workingDirectory = new StringBuilder(260);
            shellLink.GetWorkingDirectory(workingDirectory, 260);
            return workingDirectory.ToString();
        }
        set
        {
            if (value.Length > 260)
                throw new ArgumentException("The length of the new working directory path should not exceed 260.");
            shellLink.SetWorkingDirectory(value);
        }
    }

    /// <summary>
    ///     Gets or sets the startup arguments of the executable to which <see cref="ExecutablePath" /> is pointing to.
    /// </summary>
    /// <exception cref="ArgumentException">If provided description is too long.</exception>
    public string Arguments
    {
        get
        {
            var arguments = new StringBuilder(1024);
            shellLink.GetArguments(arguments, arguments.Capacity);
            return arguments.ToString();
        }
        set
        {
            if (value.Length > 1024)
                throw new ArgumentException("The length of the new arguments should not exceed 1024.");
            shellLink.SetArguments(value);
        }
    }

    /// <summary>
    ///     Gets or sets the icon location.
    /// </summary>
    /// <exception cref="ArgumentException">If provided path is longer than allowed (260 characters).</exception>
    public string IconLocation
    {
        get
        {
            var iconLocation = new StringBuilder(260);
            shellLink.GetIconLocation(iconLocation, iconLocation.Capacity, out _);
            return iconLocation.ToString();
        }
        set
        {
            if (value.Length > 260)
                throw new ArgumentException("The length of the path to a new icon should not exceed 260.");
            shellLink.SetIconLocation(value, 0);
        }
    }
}