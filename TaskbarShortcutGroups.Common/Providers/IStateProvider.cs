using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Providers;

public interface IStateProvider
{
    List<IShortcutGroup> ShortcutGroups { get; }
    void Save();
}