using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutGroupListViewModel : ViewModelBase
{
    private readonly IFactory<ShortcutGroupEditorViewModel> groupEditorFactory;

    public ShortcutGroupListViewModel(INavigationService navigationService, IStateService stateService, IFactory<ShortcutGroupEditorViewModel> groupEditorFactory)
        : base(navigationService, stateService)
    {
        this.groupEditorFactory = groupEditorFactory;
        ShortcutGroups = new(stateService.ShortcutGroups.Select(group => groupEditorFactory.Construct(group)));
    }

    public ObservableCollection<ShortcutGroupEditorViewModel> ShortcutGroups { get; }

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

    public void RemoveGroup(ShortcutGroupEditorViewModel shortcutGroup)
    {
        ShortcutGroups.Remove(shortcutGroup);
        navigationService.Navigate(shortcutGroup);
    }

    public void NavigateToGroup(ShortcutGroupEditorViewModel shortcutGroup)
    {
        navigationService.Navigate(shortcutGroup);
    }
}