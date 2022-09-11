using System.Collections.ObjectModel;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class ShortcutGroupViewModel : ViewModelBase
{
    public string Content => "ShortcutGroupView Content";

    private ShortcutGroup shortcutGroup;

    public string Name => shortcutGroup.Name;
    public ObservableCollection<Shortcut> Shortcuts => shortcutGroup.Shortcuts;
    
}