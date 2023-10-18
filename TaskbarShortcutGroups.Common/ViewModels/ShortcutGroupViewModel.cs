using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupViewModel : ViewModelBase
{
    public IEnumerable<ShortcutViewModel> Shortcuts { get; }

    public ShortcutGroupViewModel(INavigationService navigationService, IStateService stateService, IShortcutViewModelFactory shortcutFactory, IShortcutGroup shortcutGroup)
        : base(navigationService, stateService)
    {
        Shortcuts = shortcutGroup.Shortcuts.Select(shortcutFactory.Create);
    }
}