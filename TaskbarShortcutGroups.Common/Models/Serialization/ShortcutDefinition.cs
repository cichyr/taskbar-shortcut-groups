using System.Xml.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class ShortcutDefinition
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
    
    [XmlAttribute("path")]
    public string FilePath { get; set; } = string.Empty;
}