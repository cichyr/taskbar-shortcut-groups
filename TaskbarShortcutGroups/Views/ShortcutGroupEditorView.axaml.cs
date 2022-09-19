using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskbarShortcutGroups.Views;

public partial class ShortcutGroupEditorView : UserControl
{
    public ShortcutGroupEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}