namespace TaskbarShortcutGroups.Common.Providers;

public interface IVersionProvider
{
    /// <summary>
    /// Gets the product version.
    /// </summary>
    string ProductVersion { get; }
}