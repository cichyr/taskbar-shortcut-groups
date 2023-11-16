using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TaskbarShortcutGroups.AvaloniaUI;

internal static class Program
{
    private static string AssembliesPath { get; } = @$"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty}\Assemblies\";

    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        StartApp(args);
    }

    [STAThread]
    private static void StartApp(string[] args)
    {
        var appBuilder = Avalonia.AppBuilder.Configure<App>();
        Avalonia.AppBuilderDesktopExtensions.UsePlatformDetect(appBuilder);
        Avalonia.LoggingExtensions.LogToTrace(appBuilder);
        Avalonia.ClassicDesktopStyleApplicationLifetimeExtensions.StartWithClassicDesktopLifetime(appBuilder, args);
    }

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