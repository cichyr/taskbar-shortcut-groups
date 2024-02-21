using System.Drawing;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Providers;

namespace TaskbarShortcutGroups.Common.Services;

public class StateService : IStateService
{
    private readonly IShortcutFactory shortcutFactory;
    private readonly IShortcutGroupFactory shortcutGroupFactory;
    private readonly IStateProvider stateProvider;

    public StateService(IStateProvider stateProvider, IShortcutGroupFactory shortcutGroupFactory, IShortcutFactory shortcutFactory)
    {
        this.stateProvider = stateProvider ?? throw new ArgumentNullException(nameof(stateProvider));
        this.shortcutGroupFactory = shortcutGroupFactory ?? throw new ArgumentNullException(nameof(shortcutGroupFactory));
        this.shortcutFactory = shortcutFactory ?? throw new ArgumentNullException(nameof(shortcutFactory));
    }

    public event EventHandler<IShortcutGroup>? ShortcutGroupRemoved;
    public List<IShortcutGroup> ShortcutGroups => stateProvider.ShortcutGroups;

    public void SaveState()
    {
        stateProvider.Save();
        foreach (var group in ShortcutGroups)
            CreateOrUpdateShortcutForGroup(group);
        var shortcutGroupNames = ShortcutGroups.Select(x => $"{x.Name}.lnk");
        Directory
            .GetFiles(StorageLocation.Shortcuts)
            .Where(path => !shortcutGroupNames.Any(path.EndsWith))
            .ForEach(File.Delete);
    }

    public IShortcutGroup CreateGroup()
    {
        var newGroup = shortcutGroupFactory.Create();
        newGroup.Order = ShortcutGroups.Any()
            ? ShortcutGroups.Max(x => x.Order) + 1
            : 0;
        ShortcutGroups.Add(newGroup);
        return newGroup;
    }

    public IShortcut AddShortcutToGroup(IShortcutGroup group, string path)
    {
        var newShortcut = shortcutFactory.Create(path);
        newShortcut.Order = group.Shortcuts.Any()
            ? group.Shortcuts.Max(x => x.Order) + 1
            : 0;
        if (!group.Shortcuts.Add(newShortcut))
            throw new ArgumentException("Cannot add the same IShortcut twice");
        return newShortcut;
    }

    public void RemoveShortcutFromGroup(IShortcutGroup group, IShortcut shortcut)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(shortcut);
        if (!group.Shortcuts.Remove(shortcut))
            throw new ArgumentException("Shortcut is not in this group");
        shortcut.Dispose();
    }

    public void RemoveGroup(IShortcutGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        if (!ShortcutGroups.Remove(group))
            throw new ArgumentException("Shortcut group does not exist");
        ShortcutGroupRemoved?.Invoke(this, group);
        SaveState();
    }

    public void Dispose()
    {
        ShortcutGroups.ForEach(x => x.Dispose());
        GC.SuppressFinalize(this);
    }

    private void CreateOrUpdateShortcutForGroup(IShortcutGroup group)
    {
        var shortcutPath = Path.Join(StorageLocation.Shortcuts, $"{group.Name}.lnk");
        var iconPath = Path.Join(StorageLocation.Icons, $"{group.Name}.ico");
        if (File.Exists(group.IconPath))
            new Bitmap(group.IconPath)
                .ToIcon()
                .Save(iconPath, true)
                .Dispose();
        var shortcutExists = File.Exists(shortcutPath);
        using var shortcut = shortcutExists
            ? shortcutFactory.Create(shortcutPath)
            : shortcutFactory.Create();
        if (!shortcutExists)
        {
            shortcut.Name = group.Name;
            shortcut.ExecutablePath = StorageLocation.ApplicationExecutable;
            shortcut.Arguments = $"\"{group.Name}\"";
            shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
            shortcut.IconLocation = new IconLocation(iconPath, 0);
        }

        shortcut.IconLocation = new IconLocation(iconPath, 0);
        shortcut.Save(StorageLocation.Shortcuts);
    }
}