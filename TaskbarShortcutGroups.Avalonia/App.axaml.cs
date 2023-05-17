using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TaskbarShortcutGroups.Avalonia.Services;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;
using TaskbarShortcutGroups.Avalonia.Views;
using GroupWindow = TaskbarShortcutGroups.Avalonia.Views.GroupWindow;

namespace TaskbarShortcutGroups;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        SetupMainWindow(ApplicationLifetime);
        base.OnFrameworkInitializationCompleted();
    }

    private static void SetupMainWindow(IApplicationLifetime? lifetime)
    {
        IoCContainer.Container
            .RegisterSingleton<IAvaloniaNavigationService, NavigationService>()
            .RegisterSingleton<INavigationService, NavigationService>()
            .RegisterSingleton<IDialogService, DialogService>()
            .RegisterFactory<Shortcut>()
            .RegisterFactory<ShortcutGroup>()
            .RegisterSingleton<IStateService, StateService>()
            .RegisterFactory<ShortcutViewModel>()
            .RegisterFactory<ShortcutGroupViewModel>()
            .RegisterFactory<ShortcutGroupListViewModel>()
            .RegisterFactory<ShortcutGroupEditorViewModel>();
        var navigationService = (NavigationService)IoCContainer.Container.Resolve<INavigationService>();
        var stateService = IoCContainer.Container.Resolve<IStateService>();
        switch (lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                if (desktop.Args.Any())
                {
                    var shortcutGroup = stateService.ShortcutGroups.FirstOrDefault(sg => sg.Name == desktop.Args[0]);
                    //desktop.Exit += -> Add disposal of stacker groups handle
                    if (shortcutGroup is not null)
                    {
                        var groupViewModel = IoCContainer.Container
                            .Resolve<IFactory<ShortcutGroupViewModel>>()
                            .Construct(shortcutGroup);
                        desktop.MainWindow = new GroupWindow { DataContext = navigationService };
#if !DEBUG
                        desktop.MainWindow.LostFocus += (_, _) => Environment.Exit(0);
#endif
                        navigationService.Setup(desktop.MainWindow);
                        navigationService.Navigate(groupViewModel);
                        break;
                    }
                }

                var groupListViewModel = IoCContainer.Container
                    .Resolve<IFactory<ShortcutGroupListViewModel>>()
                    .Construct();
                desktop.MainWindow = new MainWindow { DataContext = navigationService };
                // desktop.ShutdownRequested += -> Add saving handle
                navigationService.Setup(desktop.MainWindow);
                navigationService.Navigate(groupListViewModel);
                break;
            default:
                throw new NotSupportedException($"Only {nameof(IClassicDesktopStyleApplicationLifetime)} lifetime is supported");
        }
    }
}