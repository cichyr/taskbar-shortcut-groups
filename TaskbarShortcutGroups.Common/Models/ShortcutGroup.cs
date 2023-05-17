using System.Xml.Serialization;

namespace TaskbarShortcutGroups.Common.Models;

public class ShortcutGroup
{
    [XmlAttribute(nameof(Name))]
    public string Name { get; set; } = string.Empty;
    [XmlAttribute(nameof(IconPath))]
    public string IconPath { get; set; } = string.Empty;
    [XmlElement(nameof(Shortcut))]
    public HashSet<Shortcut> Shortcuts { get; } = new();
}