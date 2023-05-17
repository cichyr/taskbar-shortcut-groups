using System.Linq;
using FileDialogFilter=Avalonia.Controls.FileDialogFilter;

namespace TaskbarShortcutGroups.Avalonia.Extensions;

public static class FileDialogFilterExtensions
{
    public static FileDialogFilter ToAvaloniaFilter(this TaskbarShortcutGroups.Common.Models.FileDialogFilter fileDialogFilter) =>
        new()
        {
            Name = fileDialogFilter.Name,
            Extensions = fileDialogFilter.Extensions.ToList(),
        };
}