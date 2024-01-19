namespace TaskbarShortcutGroups.Common.Providers;

public interface IVersionProvider
{
    /// <summary>
    /// Gets the product version.
    /// </summary>
    string ProductVersion { get; }

    bool UpdateDetected { get; }

    /// <summary>
    /// Gets the product version.
    /// </summary>
    string NewestVersion { get; }

    /// <summary>
    /// Event raised when a possible update is detected.
    /// </summary>
    event EventHandler? NewerVersionDetected;
}