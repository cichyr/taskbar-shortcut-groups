using System.Xml.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutGroupsDefinition
{
    [XmlArray("ShortcutGroups")]
    [XmlArrayItem("ShortcutGroup")]
    public List<ShortcutGroupDefinition> ShortcutGroups { get; set; } = new();
}