using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TaskbarShortcutGroups.AvaloniaUI.Views;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.AvaloniaUI;

public class ViewLocator : IDataTemplate
{
    private static readonly string ViewsNamespace = typeof(ViewLocator).FullName!.Replace(nameof(ViewLocator), "Views");

    public Control Build(object? data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var name = data.GetType().FullName!.Split("ViewModels").Last().Replace("ViewModel", "View");
        var fullyQualifiedViewName = $"{ViewsNamespace}{name}";
        var type = Type.GetType(fullyQualifiedViewName);
        return type == null
            ? new TextBlock {Text = "Not Found: " + name}
            : type.Name switch
            {
                nameof(AboutView) => Activator.CreateInstance<AboutView>(),
                nameof(ShortcutGroupEditorView) => Activator.CreateInstance<ShortcutGroupEditorView>(),
                nameof(ShortcutGroupListView) => Activator.CreateInstance<ShortcutGroupListView>(),
                nameof(ShortcutGroupView) => Activator.CreateInstance<ShortcutGroupView>(),
                _ => new TextBlock {Text = "Not Found: " + name},
            };
    }

    public bool Match(object? data)
        => data is ViewModelBase;
}