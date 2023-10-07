namespace TaskbarShortcutGroups.Common.Models.Licensing;

public interface ILicense
{
    string ComponentName { get; }
    string Author { get; }
    string Text { get; }
}