using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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
        var pickerOptions = new FilePickerOpenOptions
        {
            AllowMultiple = allowMultipleFiles,
            Title = title,
            FileTypeFilter = filters.Select(f => f.ToAvaloniaFilter()).ToArray()
        };
        var selectedFiles = await navigationService.CurrentWindow.StorageProvider.OpenFilePickerAsync(pickerOptions);
        return selectedFiles.Select(x => x.Path.LocalPath).ToArray();
    }
}