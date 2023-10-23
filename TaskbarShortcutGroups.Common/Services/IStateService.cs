using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public interface IStateService : IDisposable
{
    List<IShortcutGroup> ShortcutGroups { get; }
    event EventHandler<IShortcutGroup> ShortcutGroupRemoved;
    void SaveState();
    IShortcutGroup CreateGroup();
    IShortcut AddShortcutToGroup(IShortcutGroup group, string path);
    void RemoveShortcutFromGroup(IShortcutGroup group, IShortcut shortcut);
    void RemoveGroup(IShortcutGroup group);
}