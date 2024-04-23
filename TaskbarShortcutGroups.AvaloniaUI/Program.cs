using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Pure.DI;
using TaskbarShortcutGroups.AvaloniaUI.Services;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;
using TaskbarShortcutGroups.Windows.IoC.Factories;
using TaskbarShortcutGroups.Windows.Services;
using static Pure.DI.Lifetime;

namespace TaskbarShortcutGroups.AvaloniaUI;

internal class Program
{
    [Obsolete]
    private static string AssembliesPath { get; } = @$"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty}\Assemblies\";

    [STAThread]
    public static void Main(string[] args)
    {
        // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        StartApp(args);
    }
    
    [Conditional("DI")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "PureDI is given name")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Needed only for compilation")]
    private static void SetupPureDI()
    {
        DI.Setup("Composition")
            .RootBind<IShortcutFactory>().As(Singleton).To<ShortcutFactory>()
            .RootBind<IStateStore>().As(Singleton).To<StateStore>()
            .RootBind<IStateProvider>().As(Singleton).To<StateProvider>()
            .RootBind<IOsService>().As(Singleton).To<OsService>()
            .RootBind<IShortcutViewModelFactory>().As(Singleton).To<ShortcutViewModelFactory>()
            // .RootBind<ShortcutGroupViewModel>().As(Singleton).To<ShortcutGroupViewModel>()
            .Bind<INavigationService, IAvaloniaNavigationService>().As(Singleton).To<NavigationService>()
                .Root<INavigationService>().Root<IAvaloniaNavigationService>()
            .RootBind<ITaskService>().As(Singleton).To<TaskService>()
            .RootBind<IVersionProvider>().As(Singleton).To<VersionProvider>()
            .RootBind<IDialogService>().As(Singleton).To<DialogService>()
            .RootBind<ILicenseProvider>().As(Singleton).To<LicenseProvider>()
            .RootBind<IStateService>().As(Singleton).To<StateService>()
            .RootBind<AboutViewModel>().As(Singleton).To<AboutViewModel>()
            .RootBind<ShortcutGroupListViewModel>().As(Singleton).To<ShortcutGroupListViewModel>()
            .RootBind<IShortcutGroupEditorViewModelFactory>().As(Singleton).To<ShortcutGroupEditorViewModelFactory>()
            .RootBind<IShortcutGroupFactory>().As(Singleton).To<ShortcutGroupFactory>()
            .Bind<IShortcutGroup>().To<IShortcutGroup>("shortcutGroup")
            .RootBind<Func<IShortcutGroup, ShortcutGroupViewModel>>().To<Func<IShortcutGroup, ShortcutGroupViewModel>>(
                ctx => shortcutGroup =>
                {
                    ctx.Inject<ShortcutGroupViewModel>(out var shortcutGroupViewModel);
                    return shortcutGroupViewModel;
                });
    }
    
    [STAThread]
    private static void StartApp(string[] args)
    {
        var appBuilder = Avalonia.AppBuilder.Configure<App>();
        Avalonia.AppBuilderDesktopExtensions.UsePlatformDetect(appBuilder);
        Avalonia.LoggingExtensions.LogToTrace(appBuilder);
        Avalonia.ClassicDesktopStyleApplicationLifetimeExtensions.StartWithClassicDesktopLifetime(appBuilder, args);
    }

    [Obsolete("Not used")]
    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        try
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = executingAssembly.GetReferencedAssemblies();
            var assemblyNameFromReferences = referencedAssemblies.FirstOrDefault(a => a.Name == args.Name);

            if (assemblyNameFromReferences != null)
                return Assembly.Load(assemblyNameFromReferences);

            var dllName = args.Name.Split(",")[0] + ".dll";
            var alternativeAssemblyPath = Path.Combine(AssembliesPath, dllName);
            return !string.IsNullOrEmpty(alternativeAssemblyPath) && File.Exists(alternativeAssemblyPath)
                ? Assembly.LoadFrom(alternativeAssemblyPath)
                : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}