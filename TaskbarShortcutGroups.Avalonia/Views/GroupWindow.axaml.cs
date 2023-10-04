using Avalonia;
using Avalonia.Controls;
using Control = System.Windows.Forms.Control;

namespace TaskbarShortcutGroups.Avalonia.Views;

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
        Position = new PixelPoint((int)(mousePosition.X - Width / 2), (int)(mousePosition.Y - Height - 20));
        // Focus(NavigationMethod.Pointer); 
        // SendKeys.SendWait("{TAB}");
        // Focusable = true;
    }
}