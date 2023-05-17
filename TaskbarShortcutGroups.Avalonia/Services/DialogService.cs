using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using TaskbarShortcutGroups.Avalonia.Extensions;
using TaskbarShortcutGroups.Common.Services;
using FileDialogFilter = TaskbarShortcutGroups.Common.Models.FileDialogFilter;

namespace TaskbarShortcutGroups.Avalonia.Services;

public class DialogService : IDialogService
{
    private readonly IAvaloniaNavigationService navigationService;

    public DialogService(IAvaloniaNavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public async Task<string[]?> OpenFileDialog(string title, bool allowMultipleFiles, params FileDialogFilter[] filters)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = allowMultipleFiles,
            Filters = filters.Select(f => f.ToAvaloniaFilter()).ToList(),
            Title = title
        };
        return await dialog.ShowAsync(navigationService.CurrentWindow);
    }
}