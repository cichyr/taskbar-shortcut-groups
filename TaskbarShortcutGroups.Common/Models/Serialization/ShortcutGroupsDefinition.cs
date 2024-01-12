namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutGroupsDefinition
{
    public int Version { get; set; } = 1;

    public List<ShortcutGroupDefinition> ShortcutGroups { get; set; } = new();
}