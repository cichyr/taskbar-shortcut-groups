using Avalonia.Controls;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Avalonia.Services;

/// <summary>
/// The Avalonia-specific implementation of navigation service.
/// </summary>
public interface IAvaloniaNavigationService : INavigationService
{
    /// <summary>
    /// Gets the current window.
    /// </summary>
    Window CurrentWindow { get; }

    /// <summary>
    /// Setups the navigation service.
    /// </summary>
    /// <param name="window"> The window instance </param>
    void Setup(Window window);
}