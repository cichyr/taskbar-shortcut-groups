using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskbarShortcutGroups.Avalonia.Views;

public partial class ShortcutGroupView : UserControl
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