using System.Xml.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutGroupDefinitions
{
    [XmlElement(nameof(ShortcutGroup))]
    public List<ShortcutGroup> ShortcutGroups { get; set; }
}