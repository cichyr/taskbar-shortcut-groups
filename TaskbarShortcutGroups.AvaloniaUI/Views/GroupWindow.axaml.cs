using Avalonia;
using Avalonia.Controls;
using Control = System.Windows.Forms.Control;

namespace TaskbarShortcutGroups.AvaloniaUI.Views;

public partial class GroupWindow : Window
{
    public GroupWindow()
    {
        InitializeComponent();
    }

    public override void Show()
    {
        base.Show();
        var mousePosition = Control.MousePosition;
        Position = new PixelPoint((int)(mousePosition.X - Width * RenderScaling / 2), (int)(mousePosition.Y - Height * RenderScaling - 20));
    }
}