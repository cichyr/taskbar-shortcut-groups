namespace TaskbarShortcutGroups.Common.Models;

public interface IShortcutGroup : IDisposable
{
    string Name { get; set; }

    string IconPath { get; set; }

    int Order { get; set; }

    HashSet<IShortcut> Shortcuts { get; init; }
}