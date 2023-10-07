namespace TaskbarShortcutGroups.Common.Models;

public interface IShortcutGroup : IDisposable
{
    string Name { get; set; }
    string IconPath { get; set; }
    HashSet<IShortcut> Shortcuts { get; init; }
}