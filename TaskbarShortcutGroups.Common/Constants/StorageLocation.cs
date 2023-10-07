using System.Reflection;

namespace TaskbarShortcutGroups.Common.Constants;

public static class StorageLocation
{
    public static string ApplicationExecutable { get; } = Assembly.GetEntryAssembly()!.Location.Replace(".dll", ".exe");
    public static string Config { get; } = Path.Join(Directory.GetCurrentDirectory(), "Config");
    public static string Licenses { get; } = Path.Join(Directory.GetCurrentDirectory(), "Licenses");
    public static string Icons { get; } = Path.Join(Config, "Icons");
    public static string StateFile { get; } = Path.Join(Config, "ShortcutGroups.xml");
    public static string Shortcuts { get; } = Path.Join(Config, "Shortcuts");
}