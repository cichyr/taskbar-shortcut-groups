using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupListViewModel : ViewModelBase
{
    private readonly IFactory<ShortcutGroupEditorViewModel> groupEditorFactory;
    private ICommand? navigateToGroup;
    private ICommand? removeGroup;
    private AboutViewModel aboutViewModel;

    public ShortcutGroupListViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutGroupEditorViewModel> groupEditorFactory, IFactory<AboutViewModel> aboutViewModelFactory)
        : base(navigationService, stateService)
    {
        aboutViewModel = aboutViewModelFactory.Construct();
        this.groupEditorFactory = groupEditorFactory;
        ShortcutGroups = new ObservableCollection<ShortcutGroupEditorViewModel>(stateService.ShortcutGroups.Select(group => groupEditorFactory.Construct(group)));
    }

    public ObservableCollection<ShortcutGroupEditorViewModel> ShortcutGroups { get; }

    public ICommand RemoveGroup => removeGroup ??= new RelayCommand<ShortcutGroupEditorViewModel>(shortcutGroup =>
    {
        ArgumentNullException.ThrowIfNull(shortcutGroup);
        ShortcutGroups.Remove(shortcutGroup);
        stateService.RemoveGroup(shortcutGroup.InnerObject);
    });

    public ICommand NavigateToGroup => navigateToGroup ??= new RelayCommand<ShortcutGroupEditorViewModel>(shortcutGroup =>
    {
        ArgumentNullException.ThrowIfNull(shortcutGroup);
        navigationService.Navigate(shortcutGroup);
    });

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
        var shortcutGroup = groupEditorFactory.Construct();
        ShortcutGroups.Add(shortcutGroup);
        navigationService.Navigate(shortcutGroup);
    }

    public void OpenAboutView()
        => navigationService.Navigate(aboutViewModel);

    public void OpenShortcutsLocation()
        => Process.Start("explorer.exe", StorageLocation.Shortcuts);
}