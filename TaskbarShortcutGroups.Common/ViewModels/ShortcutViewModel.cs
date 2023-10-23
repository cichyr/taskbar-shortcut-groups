using System.Diagnostics;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class ShortcutViewModel : ObservableObject
{
    private readonly IShortcut innerObject;

    public ShortcutViewModel(string path, IShortcutGroup parentGroup)
    {
        innerObject = new Shortcut(path);
        parentGroup.Shortcuts.Add(innerObject);
    }

    public ShortcutViewModel(IShortcut shortcut)
    {
        innerObject = shortcut;
    }

    public string Name
    {
        get => innerObject.Name;
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

    public Bitmap? Icon => innerObject.IconBitmap;

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