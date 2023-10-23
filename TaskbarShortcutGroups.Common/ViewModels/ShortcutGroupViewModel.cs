using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupViewModel : ViewModelBase
{
    public ShortcutGroupViewModel(IShortcutViewModelFactory shortcutFactory, IShortcutGroup shortcutGroup)
    {
        Shortcuts = shortcutGroup.Shortcuts.Select(shortcutFactory.Create);
    }

    public IEnumerable<ShortcutViewModel> Shortcuts { get; }
}