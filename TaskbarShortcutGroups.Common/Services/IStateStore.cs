using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public interface IStateStore
{
    void Save(IEnumerable<IShortcutGroup> shortcutGroups);
    IEnumerable<IShortcutGroup>? Load();
}