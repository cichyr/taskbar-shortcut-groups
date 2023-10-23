using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupEditorViewModel : ViewModelBase
{
    private static readonly FileDialogFilter ImageFilter = new("Images", "*.jpg", "*.png", "*.ico");
    private static readonly FileDialogFilter ShortcutFilter = new("Shortcuts", "*.lnk");
    private readonly IShortcutViewModelFactory shortcutFactory;
    private bool isNewGroup;
    private ICommand? navigateBack;
    private ICommand? removeShortcut;
    private ICommand? saveGroup;
    private ICommand? removeGroup;

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IShortcutViewModelFactory shortcutFactory, IShortcutGroup shortcutGroup)
        : base(navigationService, stateService)
    {
        TitleNamePrefix = shortcutGroup.Name;
        this.shortcutFactory = shortcutFactory;
        InnerObject = shortcutGroup;
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            shortcutGroup.Shortcuts.Select(shortcutFactory.Create));
    }

    public ShortcutGroupEditorViewModel(INavigationService navigationService, IStateService stateService, IShortcutViewModelFactory shortcutFactory)
        : base(navigationService, stateService)
    {
        this.shortcutFactory = shortcutFactory;
        InnerObject = new ShortcutGroup();
        isNewGroup = true;
        TitleNamePrefix = "Create new shortcut group";
        Shortcuts = new ObservableCollection<ShortcutViewModel>(
            InnerObject.Shortcuts.Select(shortcutFactory.Create));
        stateService.ShortcutGroups.Add(InnerObject);
    }

    public IShortcutGroup InnerObject { get; }

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

    public ICommand SaveGroup => saveGroup ??= new RelayCommand(() =>
    {
        isNewGroup = false;
        stateService.SaveState();
    });

    public ICommand NavigateBack => navigateBack ??= new RelayCommand(() =>
    {
        if (isNewGroup)
            RemoveGroup.Execute(null);
        navigationService.NavigateBack();
    });

    public ICommand RemoveGroup => removeGroup ??= new RelayCommand(() => stateService.RemoveGroup(InnerObject));

    public ICommand RemoveShortcut => removeShortcut ??= new RelayCommand<ShortcutViewModel>(shortcutViewModel =>
    {
        ArgumentNullException.ThrowIfNull(shortcutViewModel);
        var shortcut = InnerObject.Shortcuts.First(x => x.ExecutablePath == shortcutViewModel.Path);
        Shortcuts.Remove(shortcutViewModel);
        stateService.RemoveShortcutFromGroup(InnerObject, shortcut);
        OnPropertyChanged(nameof(Shortcuts));
    });

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
                Shortcuts.Add(shortcutFactory.Create(stateService.AddShortcutToGroup(InnerObject, path)));
        OnPropertyChanged(nameof(Shortcuts));
    }
}