using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskbarShortcutGroups.Views;

public partial class ShortcutGroupView : Window
{
    public ShortcutGroupView()
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