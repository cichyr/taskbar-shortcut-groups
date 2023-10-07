using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Models.Serialization;

namespace TaskbarShortcutGroups.Common.Services;

public class StateService : IStateService
{
    private readonly IFactory<Shortcut> shortcutFactory;
    private readonly IFactory<ShortcutGroup> shortcutGroupFactory;

    public StateService(IFactory<ShortcutGroup> shortcutGroupFactory, IFactory<Shortcut> shortcutFactory)
    {
        this.shortcutGroupFactory = shortcutGroupFactory ?? throw new ArgumentNullException(nameof(shortcutGroupFactory));
        this.shortcutFactory = shortcutFactory ?? throw new ArgumentNullException(nameof(shortcutFactory));
        ShortcutGroups = new List<ShortcutGroup>();
        Directory.CreateDirectory(StorageLocation.Config);
        Directory.CreateDirectory(StorageLocation.Shortcuts);
        Directory.CreateDirectory(StorageLocation.Icons);
        LoadState();
    }
    
    public List<ShortcutGroup> ShortcutGroups { get; private set; }

    public void SaveState()
    {
        XmlSerializer serializer = new(typeof(ShortcutGroupDefinitions));
        var toSerialize = new ShortcutGroupDefinitions {ShortcutGroups = ShortcutGroups.ToList()};
        using var fileStream = new FileStream(StorageLocation.StateFile, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new XmlTextWriter(fileStream, Encoding.Default);
        writer.Formatting = Formatting.Indented;
        serializer.Serialize(writer, toSerialize);
        fileStream.Flush();
        fileStream.Close();
        foreach (var group in ShortcutGroups)
            CreateShortcutIfNotExist(group);
    }

    public void LoadState()
    {
        XmlSerializer serializer = new(typeof(ShortcutGroupDefinitions));
        if (!File.Exists(StorageLocation.StateFile))
            return;
        using var fileStream = new FileStream(StorageLocation.StateFile, FileMode.Open, FileAccess.Read);
        using var reader = new XmlTextReader(fileStream);
        var shortcutGroupDefinitions = serializer.Deserialize(reader) as ShortcutGroupDefinitions;
        fileStream.Close();
        ShortcutGroups = shortcutGroupDefinitions?.ShortcutGroups ?? new List<ShortcutGroup>();
    }

    public ShortcutGroup CreateGroup()
    {
        var newGroup = shortcutGroupFactory.Construct();
        ShortcutGroups.Add(newGroup);
        return newGroup;
    }

    public Shortcut AddShortcutToGroup(ShortcutGroup group, string path)
    {
        var newShortcut = shortcutFactory.Construct(path);
        if (!group.Shortcuts.Add(newShortcut))
            throw new ArgumentException("Cannot add the same shortcut twice");
        return newShortcut;
    }

    public void RemoveShortcutFromGroup(ShortcutGroup group, Shortcut shortcut)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(shortcut);
        if (!group.Shortcuts.Remove(shortcut))
            throw new ArgumentException("Shortcut is not in this group");
    }

    public void RemoveGroup(ShortcutGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        if (!ShortcutGroups.Remove(group))
            throw new ArgumentException("Shortcut group does not exist");
    }

    private void CreateShortcutIfNotExist(ShortcutGroup group)
    {
        var shortcutPath = Path.Join(StorageLocation.Shortcuts, $"{group.Name}.lnk");
        var iconPath = Path.Join(StorageLocation.Icons, $"{group.Name}.ico");
        new Bitmap(group.IconPath).ToIcon().Save(iconPath, true);
        var shortcut = File.Exists(shortcutPath)
            ? shortcutFactory.Construct(shortcutPath)
            : shortcutFactory.Construct();
        shortcut.Name = group.Name;
        shortcut.ExecutablePath = StorageLocation.ApplicationExecutable;
        shortcut.Arguments = $"\"{group.Name}\"";
        shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
        shortcut.IconLocation = new IconLocation(iconPath, 0);
        shortcut.Save(StorageLocation.Shortcuts);
    }
}