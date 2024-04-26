using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TaskbarShortcutGroups.AvaloniaUI.Services;
using TaskbarShortcutGroups.AvaloniaUI.Views;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.AvaloniaUI;

public class App : Application
{
    internal static Composition IoCContainer { get; } = new();

    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        SetupMainWindow(ApplicationLifetime);
        base.OnFrameworkInitializationCompleted();
    }

    private static void SetupMainWindow(IApplicationLifetime? lifetime)
    {
        switch (lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.Exit += (_, _) => IoCContainer.Dispose();
                if (desktop.Args != null && desktop.Args.Length != 0)
                {
                    var stateProvider = IoCContainer.Resolve<IStateProvider>();
                    var shortcutGroup = stateProvider.ShortcutGroups.FirstOrDefault(sg => sg.Name == desktop.Args[0]);
                    if (shortcutGroup is not null)
                    {
                        var groupViewModelFactory = IoCContainer.Resolve<Func<IShortcutGroup, ShortcutGroupViewModel>>();
                        var groupViewModel = groupViewModelFactory(shortcutGroup);
                        desktop.MainWindow = new GroupWindow
                        {
                            DataContext = groupViewModel,
                            Content = groupViewModel,
                            Focusable = true
                        };

#if !DEBUG
                        desktop.MainWindow.LostFocus += (_, _) =>
                        {
                            if (desktop.MainWindow.FocusManager!.GetFocusedElement() is null)
                                Environment.Exit(0);
                        };
#endif

                        desktop.MainWindow.Focus();
                        return;
                    }
                }

                var navigationService = IoCContainer.Resolve<IAvaloniaNavigationService>();
                var groupListViewModel = IoCContainer.Resolve<ShortcutGroupListViewModel>();
                desktop.MainWindow = new MainWindow();
                navigationService.Setup(desktop.MainWindow);
                navigationService.Navigate(groupListViewModel);
                break;
            default:
                throw new NotSupportedException($"Only {nameof(IClassicDesktopStyleApplicationLifetime)} lifetime is supported");
        }
    }
}