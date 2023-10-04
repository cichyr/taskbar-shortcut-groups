using System.Linq;
using Avalonia.Platform.Storage;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Avalonia.Extensions;

public static class FileDialogFilterExtensions
{
    public static FilePickerFileType ToAvaloniaFilter(this FileDialogFilter fileDialogFilter) =>
        new (fileDialogFilter.Name)
        {
            Patterns = fileDialogFilter.Extensions.ToList(),
        };
}