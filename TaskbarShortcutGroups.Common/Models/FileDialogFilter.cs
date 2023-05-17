namespace TaskbarShortcutGroups.Common.Models;

public class FileDialogFilter
{
    public string Name { get; }
    public string[] Extensions { get; }

    public FileDialogFilter(string name, params string[] extensions)
    {
        Name = name;
        Extensions = extensions;
    }
}