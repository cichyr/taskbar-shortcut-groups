namespace TaskbarShortcutGroups.Common.Models;

/// <summary>
/// Represents location of the icon.
/// </summary>
public class IconLocation
{
    /// <summary>
    /// </summary>
    /// <param name="filePath"> The path to the file containing the icon. </param>
    /// <param name="address"> The address of the icon in the file. </param>
    /// <exception cref="ArgumentOutOfRangeException"> When address is negative - less than 0. </exception>
    /// <exception cref="ArgumentNullException"> When the path is <see langword="null" />. </exception>
    public IconLocation(string filePath, int address)
    {
        if (address < 0) throw new ArgumentOutOfRangeException(nameof(address));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        Address = address;
    }

    /// <summary>
    /// The path to the file containing the icon.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// The address (or index) of the icon in the icon file.
    /// </summary>
    public int Address { get; }
}