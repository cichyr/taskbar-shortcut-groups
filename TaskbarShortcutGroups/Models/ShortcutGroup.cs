using System.Collections.Generic;

namespace TaskbarShortcutGroups.Models;

public class ShortcutGroup
{
    public string Name { get; set; }
    public string IconPath { get; set; }
    public List<Shortcut> Shortcuts { get; set; }
}