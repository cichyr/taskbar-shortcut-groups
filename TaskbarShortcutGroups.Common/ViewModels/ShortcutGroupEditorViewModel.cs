using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupEditorViewModel : ViewModelBase
{
    private readonly IFactory<ShortcutViewModel> shortcutFactory;
    private static readonly FileDialogFilter ImageFilter = new("Images", "*.jpg", "*.png", "*.ico");
    private static readonly FileDialogFilter ShortcutFilter = new("Shortcuts", "*.lnk");
    public ShortcutGroup InnerObject { get; }
    private ICommand? removeShortcut;

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutViewModel> shortcutFactory, ShortcutGroup shortcutGroup)
        : base(navigationService, stateService)
    {
        TitleNamePrefix = shortcutGroup.Name;
        this.shortcutFactory = shortcutFactory;
        InnerObject = shortcutGroup;
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            shortcutGroup.Shortcuts.Select(s => shortcutFactory.Construct(s)));
    }

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutViewModel> shortcutFactory)
        : base(navigationService, stateService)
    {
        this.shortcutFactory = shortcutFactory;
        InnerObject = new ShortcutGroup();
        TitleNamePrefix = "Create new shortcut group";
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            InnerObject.Shortcuts.Select(s => shortcutFactory.Construct(s)));
        stateService.ShortcutGroups.Add(InnerObject);
    }

    public string IconPath
    {
        get => InnerObject.IconPath;
        private set
        {
            InnerObject.IconPath = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ShortcutViewModel> Shortcuts { get; }

    public string Name
    {
        get => InnerObject.Name;
        set => InnerObject.Name = value;
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
                Shortcuts.Add(shortcutFactory.Construct(stateService.AddShortcutToGroup(InnerObject, path)));
        OnPropertyChanged(nameof(Shortcuts));
        stateService.SaveState();
    }

    public ICommand RemoveShortcut => removeShortcut ??= new RelayCommand<ShortcutViewModel>(shortcutViewModel =>
    {
        ArgumentNullException.ThrowIfNull(shortcutViewModel);
        var shortcut = InnerObject.Shortcuts.First(x => x.ExecutablePath == shortcutViewModel.Path);
        Shortcuts.Remove(shortcutViewModel);
        stateService.RemoveShortcutFromGroup(InnerObject, shortcut);
        OnPropertyChanged(nameof(Shortcuts));
    });
}