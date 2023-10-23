using System.Drawing;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC.Factories;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public class StateService : IStateService
{
    private readonly IShortcutFactory shortcutFactory;
    private readonly IShortcutGroupFactory shortcutGroupFactory;
    private readonly IStateStore stateStore;

    public StateService(IStateStore stateStore, IShortcutGroupFactory shortcutGroupFactory, IShortcutFactory shortcutFactory)
    {
        this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        this.shortcutGroupFactory = shortcutGroupFactory ?? throw new ArgumentNullException(nameof(shortcutGroupFactory));
        this.shortcutFactory = shortcutFactory ?? throw new ArgumentNullException(nameof(shortcutFactory));
        ShortcutGroups = new List<IShortcutGroup>();
        Directory.CreateDirectory(StorageLocation.Config);
        Directory.CreateDirectory(StorageLocation.Shortcuts);
        Directory.CreateDirectory(StorageLocation.Icons);
        LoadState();
    }

    public event EventHandler<IShortcutGroup>? ShortcutGroupRemoved;
    public List<IShortcutGroup> ShortcutGroups { get; private set; }

    public void SaveState()
    {
        stateStore.Save(ShortcutGroups);
        foreach (var group in ShortcutGroups)
            CreateOrUpdateShortcutForGroup(group);
        var shortcutGroupNames = ShortcutGroups.Select(x => $"{x.Name}.lnk");
        Directory
            .GetFiles(StorageLocation.Shortcuts)
            .Where(path => !shortcutGroupNames.Any(path.EndsWith))
            .ForEach(File.Delete);
    }

    public void LoadState()
        => ShortcutGroups = stateStore.Load()?.ToList() ?? new List<IShortcutGroup>();

    public IShortcutGroup CreateGroup()
    {
        var newGroup = shortcutGroupFactory.Create();
        ShortcutGroups.Add(newGroup);
        return newGroup;
    }

    public IShortcut AddShortcutToGroup(IShortcutGroup group, string path)
    {
        var newShortcut = shortcutFactory.Create(path);
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
        new Bitmap(group.IconPath).ToIcon().Save(iconPath, true).Dispose();
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