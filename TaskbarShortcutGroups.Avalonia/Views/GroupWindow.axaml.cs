using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskbarShortcutGroups.Avalonia.Views;

public partial class GroupWindow : Window
{
    public GroupWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}