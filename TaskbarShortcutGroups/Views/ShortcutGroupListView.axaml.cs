using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TaskbarShortcutGroups.ViewModels;

namespace TaskbarShortcutGroups.Views;

public partial class ShortcutGroupListView : UserControl
{
    public ShortcutGroupListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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
        // var result = await dialog.ShowAsync(this);
        // return result?.First() ?? string.Empty;
        return default;
    }

    public async void ShowDialog(object sender, RoutedEventArgs args)
    {
        var ownerWindow = this;
        var newWindow = new ShortcutGroupView();
        newWindow.SystemDecorations = SystemDecorations.None;
        newWindow.DataContext = new ShortcutGroupViewModel();
        // await newWindow.ShowDialog(ownerWindow);
    }
}