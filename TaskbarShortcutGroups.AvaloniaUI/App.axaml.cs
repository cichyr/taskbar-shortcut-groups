using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using TaskbarShortcutGroups.AvaloniaUI.Services;
using TaskbarShortcutGroups.AvaloniaUI.Views;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;
using TaskbarShortcutGroups.Windows.IoC.Factories;

namespace TaskbarShortcutGroups.AvaloniaUI;

public class App : Application
{
    public static IContainer IoCContainer { get; } = new Container();

    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        SetupMainWindow(ApplicationLifetime);
        base.OnFrameworkInitializationCompleted();
    }

    private static void PerformExtendedIoCSetup()
    {
        IoCContainer.RegisterMany<NavigationService>(Reuse.Singleton);
        IoCContainer.Register<IVersionProvider, VersionProvider>(Reuse.Singleton);
        IoCContainer.Register<IDialogService, DialogService>(Reuse.Singleton);
        IoCContainer.Register<ILicenseProvider, LicenseProvider>(Reuse.Singleton);
        IoCContainer.Register<IStateService, StateService>(Reuse.Singleton);
        IoCContainer.Register<AboutViewModel>(Reuse.Singleton);
        IoCContainer.Register<ShortcutGroupListViewModel>(Reuse.Singleton);
        IoCContainer.Register<IShortcutGroupEditorViewModelFactory, ShortcutGroupEditorViewModelFactory>(Reuse.Singleton);
        IoCContainer.Register<IShortcutFactory, ShortcutFactory>(Reuse.Singleton);
        IoCContainer.Register<IShortcutGroupFactory, ShortcutGroupFactory>(Reuse.Singleton);
    }

    private static void PerformBasicIoCSetup()
    {
        IoCContainer.Register<IStateStore, StateStore>(Reuse.Singleton);
        IoCContainer.Register<IStateProvider, StateProvider>(Reuse.Singleton);
        IoCContainer.Register<IShortcutViewModelFactory, ShortcutViewModelFactory>(Reuse.Singleton);
        IoCContainer.Register<ShortcutGroupViewModel>(Reuse.Singleton);
    }

    private static void SetupMainWindow(IApplicationLifetime? lifetime)
    {
        PerformBasicIoCSetup();

        switch (lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.Exit += (_, _) => IoCContainer.Dispose();
                if (desktop.Args != null && desktop.Args.Any())
                {
                    var stateProvider = IoCContainer.Resolve<IStateProvider>();
                    var shortcutGroup = stateProvider.ShortcutGroups.FirstOrDefault(sg => sg.Name == desktop.Args[0]);
                    if (shortcutGroup is not null)
                    {
                        var groupViewModelFactory = IoCContainer.Resolve<Func<IShortcutGroup, ShortcutGroupViewModel>>();
                        var groupViewModel = groupViewModelFactory(shortcutGroup);
                        desktop.MainWindow = new GroupWindow {DataContext = groupViewModel};
                        desktop.MainWindow.Content = groupViewModel;
#if !DEBUG
                        desktop.MainWindow.LostFocus += (_,_) =>
                        {
                            if (desktop.MainWindow.FocusManager!.GetFocusedElement() is null)
                                Environment.Exit(0);
                        };
#endif
                        desktop.MainWindow.Focusable = true;
                        desktop.MainWindow.Focus();
                        return;
                    }
                }

                PerformExtendedIoCSetup();
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