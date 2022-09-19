using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class ShortcutGroupEditorViewModel : ViewModelBase
{
    private bool isNew;

    public ShortcutGroupEditorViewModel(ShortcutGroup shortcutGroup)
    {
        isNew = false;
        TitleNamePrefix = shortcutGroup.Name;
    }

    public ShortcutGroupEditorViewModel()
    {
        isNew = true;
        TitleNamePrefix = "Create new shortcut group";
    }
}