using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
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
        return type != null
            ? (Control)Activator.CreateInstance(type)!
            : new TextBlock {Text = "Not Found: " + name};
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}