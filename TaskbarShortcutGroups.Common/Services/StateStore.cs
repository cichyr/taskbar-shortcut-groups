using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Models.Serialization;

namespace TaskbarShortcutGroups.Common.Services;

public class StateStore : IStateStore
{
    public void Save(IEnumerable<IShortcutGroup> shortcutGroups)
    {
        var shortcutGroupsDefinition = new ShortcutGroupsDefinition();
        foreach (var shortcutGroup in shortcutGroups)
        {
            var shortcutGroupDefinition = new ShortcutGroupDefinition
            {
                Name = shortcutGroup.Name,
                IconPath = shortcutGroup.IconPath
            };
            
            shortcutGroup.Shortcuts
                .Select(shortcut => new ShortcutDefinition
                {
                    Name = shortcut.Name,
                    FilePath = shortcut.Location
                })
                .ForEach(s => shortcutGroupDefinition.Shortcuts.Add(s));
            
            shortcutGroupsDefinition.ShortcutGroups.Add(shortcutGroupDefinition);
        }
        
        XmlSerializer serializer = new(typeof(ShortcutGroupsDefinition));
        using var fileStream = new FileStream(StorageLocation.StateFile, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new XmlTextWriter(fileStream, Encoding.Default);
        writer.Formatting = Formatting.Indented;
        serializer.Serialize(writer, shortcutGroupsDefinition);
        fileStream.Flush();
        fileStream.Close();
    }

    public IEnumerable<IShortcutGroup>? Load()
    {
        XmlSerializer serializer = new(typeof(ShortcutGroupsDefinition));
        if (!File.Exists(StorageLocation.StateFile))
            return null;
        using var fileStream = new FileStream(StorageLocation.StateFile, FileMode.Open, FileAccess.Read);
        using var reader = new XmlTextReader(fileStream);
        var shortcutGroupsDefinition = serializer.Deserialize(reader) as ShortcutGroupsDefinition;
        fileStream.Close();

        return shortcutGroupsDefinition?.ShortcutGroups
            .Select(sgd => new ShortcutGroup
            {
                Name = sgd.Name,
                IconPath = sgd.IconPath,
                Shortcuts = sgd.Shortcuts.Select(sd => new Shortcut(sd.FilePath) as IShortcut).ToHashSet(),
            });
    }
}