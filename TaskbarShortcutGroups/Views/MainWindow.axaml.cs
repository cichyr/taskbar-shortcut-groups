using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TaskbarShortcutGroups.ViewModels;

namespace TaskbarShortcutGroups.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Width = 600;
        Height = 400;
    }

    public async Task<string> OpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
                {new() {Extensions = new List<string> {"lnk", "exe"}, Name = "Executables and shortcuts"}},
            Title = "Pick shortcut"
        };
        var result = await dialog.ShowAsync(this);
        return result?.First() ?? string.Empty;
    }

    public async void Browse_Clicked(object sender, RoutedEventArgs args)
    {
        var path = await OpenFileDialog();
        var context = DataContext as MainWindowViewModel;
        context!.OpenShortcut(path);
    }

    public async void ShowDialog(object sender, RoutedEventArgs args)
    {
        var ownerWindow = this;
        var newWindow = new ShortcutGroupView();
        newWindow.SystemDecorations = SystemDecorations.None;
        newWindow.DataContext = new ShortcutGroupViewModel();
        await newWindow.ShowDialog(ownerWindow);
    }
}