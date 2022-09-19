using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class ShortcutGroupListViewModel : ViewModelBase
{
    public ShortcutGroupListViewModel()
    {
        ShortcutGroups = new ObservableCollection<ShortcutGroup>();
    }

    public string Greeting { get; } = "Welcome to Taskbar Shortcut Groups!";

    public ObservableCollection<ShortcutGroup> ShortcutGroups { get; }

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
        navigationService.Navigate<ShortcutGroupEditorViewModel>();
    }
}