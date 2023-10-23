using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupListViewModel : ViewModelBase
{
    private readonly AboutViewModel aboutViewModel;
    private readonly IShortcutGroupEditorViewModelFactory groupEditorFactory;
    private readonly INavigationService navigationService;
    private readonly IStateService stateService;
    private ICommand? navigateToGroup;
    private ICommand? removeGroup;

    public ShortcutGroupListViewModel(INavigationService navigationService, IStateService stateService, IShortcutGroupEditorViewModelFactory groupEditorFactory, AboutViewModel aboutViewModel)
    {
        this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        this.stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        this.aboutViewModel = aboutViewModel ?? throw new ArgumentNullException(nameof(aboutViewModel));
        this.groupEditorFactory = groupEditorFactory ?? throw new ArgumentNullException(nameof(groupEditorFactory));
        ShortcutGroups = new ObservableCollection<ShortcutGroupEditorViewModel>(stateService.ShortcutGroups.Select(groupEditorFactory.Create));
        this.stateService.ShortcutGroupRemoved += HandleShortcutGroupRemoved;
    }

    public ObservableCollection<ShortcutGroupEditorViewModel> ShortcutGroups { get; }

    public ICommand RemoveGroup => removeGroup ??= new RelayCommand<ShortcutGroupEditorViewModel>(
        shortcutGroup =>
        {
            ArgumentNullException.ThrowIfNull(shortcutGroup);
            ShortcutGroups.Remove(shortcutGroup);
            stateService.RemoveGroup(shortcutGroup.InnerObject);
        });

    public ICommand NavigateToGroup => navigateToGroup ??= new RelayCommand<ShortcutGroupEditorViewModel>(
        shortcutGroup =>
        {
            ArgumentNullException.ThrowIfNull(shortcutGroup);
            navigationService.Navigate(shortcutGroup);
        });

    private void HandleShortcutGroupRemoved(object? sender, IShortcutGroup removedShortcutGroup)
    {
        var viewModelToRemove = ShortcutGroups.First(x => x.InnerObject == removedShortcutGroup);
        ShortcutGroups.Remove(viewModelToRemove);
    }

    public void OpenShortcut(string path)
    {
        var sc = new Shortcut(path);
        var process = new ProcessStartInfo();
        process.Arguments = sc.Arguments;
        process.FileName = sc.ExecutablePath;
        process.WorkingDirectory = sc.WorkingDirectory;
        process.UseShellExecute = true;
        process.WindowStyle = sc.WindowStyle;
        Process.Start(process);
    }

    public void AddNewGroup()
    {
        var shortcutGroup = groupEditorFactory.Create();
        ShortcutGroups.Add(shortcutGroup);
        navigationService.Navigate(shortcutGroup);
    }

    public void OpenAboutView()
        => navigationService.Navigate(aboutViewModel);

    public void OpenShortcutsLocation()
        => Process.Start("explorer.exe", StorageLocation.Shortcuts);
}