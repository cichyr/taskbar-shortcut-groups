namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutDefinition
{
    public string Name { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public int Order { get; set; } = 0;
}