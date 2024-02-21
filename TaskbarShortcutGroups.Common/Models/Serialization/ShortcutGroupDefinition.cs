namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutGroupDefinition
{
    public string Name { get; set; } = string.Empty;

    public string IconPath { get; set; } = string.Empty;

    public int Order { get; set; } = 0;

    public List<ShortcutDefinition> Shortcuts { get; set; } = new();
}