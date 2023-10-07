using System.Xml.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutGroupDefinition
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
    
    [XmlAttribute("iconPath")]
    public string IconPath { get; set; } = string.Empty;
    
    [XmlArray("Shortcuts")]
    [XmlArrayItem("Shortcut")]
    public List<ShortcutDefinition> Shortcuts { get; } = new();
}