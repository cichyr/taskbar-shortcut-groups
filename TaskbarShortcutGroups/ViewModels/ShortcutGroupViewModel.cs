using System.Collections.ObjectModel;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class ShortcutGroupViewModel : ViewModelBase
{
    private ShortcutGroup shortcutGroup;
    public string Content => "ShortcutGroupView Content";

    public string Name => shortcutGroup.Name;
    public ObservableCollection<Shortcut> Shortcuts => new(shortcutGroup.Shortcuts);

    public void CreateNewGroup(string groupName)
    {
    }
}