using System.Text.Json;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Models.Serialization;

namespace TaskbarShortcutGroups.Common.Services;

public class StateStore : IStateStore
{
    private readonly IShortcutFactory shortcutFactory;

    public StateStore(IShortcutFactory shortcutFactory)
    {
        this.shortcutFactory = shortcutFactory ?? throw new ArgumentNullException(nameof(shortcutFactory));
    }

    public void Save(IEnumerable<IShortcutGroup> shortcutGroups)
    {
        var shortcutGroupsDefinition = new ShortcutGroupsDefinition();
        foreach (var shortcutGroup in shortcutGroups)
        {
            var shortcutGroupDefinition = new ShortcutGroupDefinition
            {
                Name = shortcutGroup.Name,
                Order = shortcutGroup.Order,
                IconPath = shortcutGroup.IconPath
            };

            shortcutGroup.Shortcuts
                .Select(shortcut => new ShortcutDefinition
                {
                    Name = shortcut.Name,
                    Order = shortcut.Order,
                    FilePath = shortcut.Location
                })
                .ForEach(shortcutGroupDefinition.Shortcuts.Add);

            shortcutGroupsDefinition.ShortcutGroups.Add(shortcutGroupDefinition);
        }

        using var fileStream = new FileStream(StorageLocation.StateFile, FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(fileStream, shortcutGroupsDefinition, SourceGenerationContext.Default.ShortcutGroupsDefinition);
        fileStream.Flush();
    }

    public IEnumerable<IShortcutGroup>? Load()
    {
        if (!File.Exists(StorageLocation.StateFile))
            return null;

        using var reader = new StreamReader(StorageLocation.StateFile);
        var shortcutGroupsDefinition = JsonSerializer.Deserialize(reader.BaseStream, SourceGenerationContext.Default.ShortcutGroupsDefinition);

        return shortcutGroupsDefinition?.ShortcutGroups
            .Select(sgd => new ShortcutGroup
            {
                Name = sgd.Name,
                IconPath = sgd.IconPath,
                Shortcuts = sgd.Shortcuts
                    .Select(CreateShortcut)
                    .OrderBy(s => s.Order)
                    .ToHashSet()
            })
            .OrderBy(sg => sg.Order);
    }

    private IShortcut CreateShortcut(ShortcutDefinition shortcutDefinition)
    {
        var shortcut = shortcutFactory.Create(shortcutDefinition.FilePath);
        shortcut.Order = shortcutDefinition.Order;
        return shortcut;
    }
}