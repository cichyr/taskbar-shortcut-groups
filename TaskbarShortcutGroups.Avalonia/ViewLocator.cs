using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.Avalonia;

public class ViewLocator : IDataTemplate
{
    private static string viewsNamespace = typeof(ViewLocator).FullName!.Replace(nameof(ViewLocator), "Views");
    
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Split("ViewModels").Last().Replace("ViewModel", "View");
        var fullyQualifiedViewName = $"{viewsNamespace}{name}";
        var type = Type.GetType(fullyQualifiedViewName);
        return type != null
            ? (Control)Activator.CreateInstance(type)!
            : new TextBlock {Text = "Not Found: " + name};
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}