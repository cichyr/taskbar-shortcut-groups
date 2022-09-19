using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TaskbarShortcutGroups.Services;
using TaskbarShortcutGroups.ViewModels;
using TaskbarShortcutGroups.Views;

namespace TaskbarShortcutGroups;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var window = new MainWindow { DataContext = NavigationService.Instance};
        NavigationService.Instance.Setup(window);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = window;
        NavigationService.Instance.Navigate<ShortcutGroupListViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}