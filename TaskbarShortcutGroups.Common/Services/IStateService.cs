using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public interface IStateService
{
    List<ShortcutGroup> ShortcutGroups { get; }
    void SaveState();
    void LoadState();
    ShortcutGroup CreateGroup();
    Shortcut AddShortcutToGroup(ShortcutGroup group, string path);
    void RemoveShortcutFromGroup(ShortcutGroup group, Shortcut shortcut);
}