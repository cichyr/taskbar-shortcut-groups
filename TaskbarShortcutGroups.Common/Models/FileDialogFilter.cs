namespace TaskbarShortcutGroups.Common.Models;

public class FileDialogFilter
{
    public FileDialogFilter(string name, params string[] extensions)
    {
        Name = name;
        Extensions = extensions;
    }

    public string Name { get; }
    public string[] Extensions { get; }
}