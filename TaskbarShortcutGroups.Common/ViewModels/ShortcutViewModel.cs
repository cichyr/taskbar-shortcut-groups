using System.Diagnostics;
using System.Drawing;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutViewModel : ViewModelBase
{
    private readonly Shortcut innerObject;

    public ShortcutViewModel(INavigationService navigationService, IStateService stateService, string path, ShortcutGroup parentGroup)
        : base(navigationService, stateService)
    {
        innerObject = new Shortcut(path);
        parentGroup.Shortcuts.Add(innerObject);
    }

    public ShortcutViewModel(INavigationService navigationService, IStateService stateService, Shortcut shortcut)
        : base(navigationService, stateService)
    {
        innerObject = shortcut;
    }

    public string Name
    {
        get => innerObject.Name ?? string.Empty;
        set
        {
            if (innerObject.Name == value) return;
            innerObject.Name = value;
            OnPropertyChanged();
        }
    }

    public string Path
    {
        get => innerObject.ExecutablePath;
        set
        {
            if (innerObject.ExecutablePath == value) return;
            innerObject.ExecutablePath = value;
            OnPropertyChanged();
        }
    }

    public Bitmap Icon => innerObject.IconBitmap;

    public void RunAndStopApplication()
    {
        var process = new ProcessStartInfo();
        process.Arguments = innerObject.Arguments;
        process.FileName = innerObject.ExecutablePath;
        process.WorkingDirectory = innerObject.WorkingDirectory;
        process.UseShellExecute = true;
        process.WindowStyle = innerObject.WindowStyle;
        Process.Start(process);
        Environment.Exit(0);
    }
}