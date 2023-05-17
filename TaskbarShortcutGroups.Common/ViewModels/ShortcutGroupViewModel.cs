using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupViewModel : ViewModelBase
{
    private readonly ShortcutGroup innerObject;

    public string Name => innerObject.Name;
    public IEnumerable<ShortcutViewModel> Shortcuts { get; }

    public ShortcutGroupViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutViewModel> shortcutFactory, ShortcutGroup shortcutGroup)
        : base(navigationService, stateService)
    {
        innerObject = shortcutGroup;
        Shortcuts = innerObject.Shortcuts.Select(s => shortcutFactory.Construct(s));
    }
}