using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskbarShortcutGroups.Views;

public partial class ShortcutGroupView : Window
{
    public ShortcutGroupView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}