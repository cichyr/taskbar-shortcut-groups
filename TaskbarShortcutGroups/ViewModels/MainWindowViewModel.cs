using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Taskbar Shortcut Groups!";

    public ObservableCollection<ShortcutGroup> ShortcutGroups { get; }

    public MainWindowViewModel()
    {
        ShortcutGroups = new();
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
}