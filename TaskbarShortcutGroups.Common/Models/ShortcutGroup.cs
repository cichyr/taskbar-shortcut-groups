using TaskbarShortcutGroups.Common.Extensions;

namespace TaskbarShortcutGroups.Common.Models;

public class ShortcutGroup : IShortcutGroup
{
    public string Name { get; set; } = string.Empty;

    public string IconPath { get; set; } = string.Empty;

    public HashSet<IShortcut> Shortcuts { get; init; } = new();

    public int Order { get; set; }

    public void Dispose()
    {
        Shortcuts.ForEach(x => x.Dispose());
        GC.SuppressFinalize(this);
    }
}