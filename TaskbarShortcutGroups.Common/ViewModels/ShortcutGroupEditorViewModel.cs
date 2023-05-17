using System.Collections.ObjectModel;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupEditorViewModel : ViewModelBase
{
    private readonly IFactory<ShortcutViewModel> shortcutFactory;
    private static readonly FileDialogFilter ImageFilter = new("Images", "jpg", "png", "ico");
    private static readonly FileDialogFilter ShortcutFilter = new("Shortcuts", "lnk");
    private readonly ShortcutGroup innerObject;

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutViewModel> shortcutFactory, ShortcutGroup shortcutGroup)
        : base(navigationService, stateService)
    {
        TitleNamePrefix = shortcutGroup.Name;
        this.shortcutFactory = shortcutFactory;
        innerObject = shortcutGroup;
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            shortcutGroup.Shortcuts.Select(s => shortcutFactory.Construct(s)));
    }

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutViewModel> shortcutFactory)
        : base(navigationService, stateService)
    {
        this.shortcutFactory = shortcutFactory;
        innerObject = new ShortcutGroup();
        TitleNamePrefix = "Create new shortcut group";
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            innerObject.Shortcuts.Select(s => shortcutFactory.Construct(s)));
        stateService.ShortcutGroups.Add(innerObject);
    }

    public string IconPath
    {
        get => innerObject.IconPath;
        private set
        {
            innerObject.IconPath = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ShortcutViewModel> Shortcuts { get; }

    public string Name
    {
        get => innerObject.Name;
        set => innerObject.Name = value;
    }

    public async Task SelectIcon()
    {
        var iconPaths = await navigationService.OpenFileDialog("Select icon", false, ImageFilter);
        if (iconPaths != null && iconPaths.Any())
            IconPath = iconPaths.First();
    }

    public async Task SelectShortcut()
    {
        var shortcutPaths = await navigationService.OpenFileDialog("Select shortcut", false, ShortcutFilter);
        if (shortcutPaths != null && shortcutPaths.Any())
            foreach (var path in shortcutPaths)
                Shortcuts.Add(shortcutFactory.Construct(stateService.AddShortcutToGroup(innerObject, path)));
        OnPropertyChanged(nameof(Shortcuts));
        stateService.SaveState();
    }

    public void RemoveShortcut(ShortcutViewModel shortcutViewModel)
    {
        var shortcut = innerObject.Shortcuts.First(x => x.ExecutablePath == shortcutViewModel.Path);
        Shortcuts.Remove(shortcutViewModel);
        stateService.RemoveShortcutFromGroup(innerObject, shortcut);
        OnPropertyChanged(nameof(Shortcuts));
    }
}