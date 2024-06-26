namespace TaskbarShortcutGroups.Common.Constants;

public static class StorageLocation
{
    public static string ApplicationExecutable { get; } = Environment.ProcessPath!;
    public static string Config { get; } = Path.Join(Directory.GetCurrentDirectory(), "Config");
    public static string Licenses { get; } = Path.Join(Directory.GetCurrentDirectory(), "Licenses");
    public static string Icons { get; } = Path.Join(Config, "Icons");
    public static string StateFile { get; } = Path.Join(Config, "ShortcutGroups.json");
    public static string Shortcuts { get; } = Path.Join(Config, "Shortcuts");
}