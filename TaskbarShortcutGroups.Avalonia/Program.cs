using System;
using Avalonia;

namespace TaskbarShortcutGroups.Avalonia;

/// <summary>
///     The main program class.
/// </summary>
internal static class Program
{
    /// <summary>
    ///     The main startup method.
    /// </summary>
    /// <remarks>
    ///     Initialization code. Don't use any Avalonia, third-party APIs or any
    ///     SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    ///     yet and stuff might break.
    /// </remarks>
    /// <param name="args"> The startup commands of the application. </param>
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    ///     Avalonia configuration; also used by visual designer.
    /// </summary>
    /// <returns> The app builder. </returns>
    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}