using System.Drawing;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public class StateService : IStateService
{
    private readonly IFactory<IShortcut> shortcutFactory;
    private readonly IFactory<IShortcutGroup> shortcutGroupFactory;
    private readonly IStateStore stateStore;

    public StateService(IStateStore stateStore, IFactory<IShortcutGroup> shortcutGroupFactory, IFactory<IShortcut> shortcutFactory)
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
        var newGroup = shortcutGroupFactory.Construct();
        ShortcutGroups.Add(newGroup);
        return newGroup;
    }

    public IShortcut AddShortcutToGroup(IShortcutGroup group, string path)
    {
        var newShortcut = shortcutFactory.Construct(path);
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
    }

    private void CreateOrUpdateShortcutForGroup(IShortcutGroup group)
    {
        var shortcutPath = Path.Join(StorageLocation.Shortcuts, $"{group.Name}.lnk");
        var iconPath = Path.Join(StorageLocation.Icons, $"{group.Name}.ico");
        new Bitmap(group.IconPath).ToIcon().Save(iconPath, true).Dispose();
        IShortcut shortcut;
        if (File.Exists(shortcutPath))
        {
            shortcut = shortcutFactory.Construct(shortcutPath);
            shortcut.IconLocation = new IconLocation(iconPath, 0);
        }
        else
        {
            shortcut = shortcutFactory.Construct();
            shortcut.Name = group.Name;
            shortcut.ExecutablePath = StorageLocation.ApplicationExecutable;
            shortcut.Arguments = $"\"{group.Name}\"";
            shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
            shortcut.IconLocation = new IconLocation(iconPath, 0);
        }

        shortcut.Save(StorageLocation.Shortcuts);
        shortcut.Dispose();
    }

    public void Dispose()
    {
        ShortcutGroups.ForEach(x => x.Dispose());
        GC.SuppressFinalize(this);
    }
}