using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace TaskbarShortcutGroups;

/// <summary>
///     The main program class.
/// </summary>
internal class Program
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
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}