using Avalonia.Controls;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //var x = new ShellLinkService();
        var sc = new Shortcut("C:/Users/wolny/source/repos/TaskbarShortcutGroups/Shortcut.lnk");
    }
}