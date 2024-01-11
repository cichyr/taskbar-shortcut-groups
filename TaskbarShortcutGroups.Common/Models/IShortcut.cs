using System.Diagnostics;
using System.Drawing;

namespace TaskbarShortcutGroups.Common.Models;

public interface IShortcut : IDisposable
{
    /// <summary>
    /// Gets the shortcut icon bitmap.
    /// </summary>
    Bitmap? IconBitmap { get; }

    /// <summary>
    /// Gets or sets the name of the shortcut.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the shortcut.
    /// </summary>
    string Location { get; set; }

    /// <summary>
    /// Gets or sets path to the executable.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    string ExecutablePath { get; set; }

    /// <summary>
    /// Gets or sets the window style of the executed shortcut.
    /// </summary>
    /// <exception cref="ArgumentException"> When provided with unsupported <see cref="ProcessWindowStyle" /> value. </exception>
    ProcessWindowStyle WindowStyle { get; set; }

    /// <summary>
    /// Gets or sets the working directory.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    string WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the startup arguments of the executable to which <see cref="ExecutablePath" /> is pointing to.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided description is too long. </exception>
    string Arguments { get; set; }

    /// <summary>
    /// Gets or sets the icon location.
    /// </summary>
    /// <exception cref="ArgumentException"> If provided path is longer than allowed (260 characters). </exception>
    IconLocation? IconLocation { get; set; }

    /// <summary>
    /// Saves the shortcut in provided path.
    /// </summary>
    /// <param name="path"> The path to the saving directory. </param>
    void Save(string path);
}