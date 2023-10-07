namespace TaskbarShortcutGroups.Common.Models.Licensing;

public class License : ILicense
{
    public string ComponentName { get; init; } = null!;

    public string Author { get; init; } = null!;

    public string Text { get; init; } = null!;
}