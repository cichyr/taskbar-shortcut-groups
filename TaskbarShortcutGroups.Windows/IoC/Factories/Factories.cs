using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Windows.Models;

namespace TaskbarShortcutGroups.Windows.IoC.Factories;

public class ShortcutFactory : IShortcutFactory
{
    public ShortcutFactory()
    {
    }
    
    public IShortcut Create()
        => new Shortcut();

    public IShortcut Create(string path)
        => new Shortcut(path);
}