using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using TaskbarShortcutGroups.Avalonia.Services;
using TaskbarShortcutGroups.Avalonia.Views;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.Avalonia;

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
        var container = new Container();
        container.RegisterMany<NavigationService>(Reuse.Singleton);
        container.Register<IVersionProvider, VersionProvider>(Reuse.Singleton);
        container.Register<IDialogService, DialogService>(Reuse.Singleton);
        container.Register<ILicenseProvider, LicenseProvider>(Reuse.Singleton);
        container.Register<IStateStore, StateStore>(Reuse.Singleton);
        container.Register<IStateService, StateService>(Reuse.Singleton);
        container.Register<AboutViewModel>(Reuse.Singleton);
        container.Register<ShortcutGroupViewModel>(Reuse.Singleton);
        container.Register<ShortcutGroupListViewModel>(Reuse.Singleton);
        container.Register<IShortcutViewModelFactory, ShortcutViewModelFactory>(Reuse.Singleton);
        container.Register<IShortcutGroupEditorViewModelFactory, ShortcutGroupEditorViewModelFactory>(Reuse.Singleton);
        container.Register<IShortcutFactory, ShortcutFactory>(Reuse.Singleton);
        container.Register<IShortcutGroupFactory, ShortcutGroupFactory>(Reuse.Singleton);

        var navigationService = container.Resolve<IAvaloniaNavigationService>();
        switch (lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.Exit += (_, _) => container.Dispose();
                if (desktop.Args != null && desktop.Args.Any())
                {
                    var stateService = container.Resolve<IStateService>();
                    var shortcutGroup = stateService.ShortcutGroups.FirstOrDefault(sg => sg.Name == desktop.Args[0]);
                    if (shortcutGroup is not null)
                    {
                        var groupViewModelFactory = container.Resolve<Func<IShortcutGroup, ShortcutGroupViewModel>>();
                        var groupViewModel = groupViewModelFactory(shortcutGroup);
                        desktop.MainWindow = new GroupWindow { DataContext = navigationService };
#if !DEBUG
                        desktop.MainWindow.LostFocus += (_,_) =>
                        {
                            if (desktop.MainWindow.FocusManager!.GetFocusedElement() is null)
                                Environment.Exit(0);
                        };
#endif
                        desktop.MainWindow.Focusable = true;
                        desktop.MainWindow.Focus();
                        navigationService.Setup(desktop.MainWindow);
                        navigationService.Navigate(groupViewModel);
                        return;
                    }
                }

                var groupListViewModel = container.Resolve<ShortcutGroupListViewModel>();
                desktop.MainWindow = new MainWindow();
                navigationService.Setup(desktop.MainWindow);
                navigationService.Navigate(groupListViewModel);
                break;
            default:
                throw new NotSupportedException($"Only {nameof(IClassicDesktopStyleApplicationLifetime)} lifetime is supported");
        }
    }
}