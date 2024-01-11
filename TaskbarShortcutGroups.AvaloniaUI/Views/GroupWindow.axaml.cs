using Avalonia;
using Avalonia.Controls;
using DryIoc;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.AvaloniaUI.Views;

public partial class GroupWindow : Window
{
    private readonly IOsService osService;

    public GroupWindow()
    {
        osService = App.IoCContainer.Resolve<IOsService>();
        InitializeComponent();
    }

    public override void Show()
    {
        base.Show();
        var mousePosition = osService.GetCursorPosition();
        Position = new PixelPoint((int)(mousePosition.X - Width * RenderScaling / 2), (int)(mousePosition.Y - Height * RenderScaling - 20));
    }
}