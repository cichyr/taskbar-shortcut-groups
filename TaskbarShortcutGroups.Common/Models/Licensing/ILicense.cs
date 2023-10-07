namespace TaskbarShortcutGroups.Common.Models.Licensing;

public interface ILicense
{
    string ComponentName { get; init; }
    string Author { get; init; }
    string Text { get; init; }
}