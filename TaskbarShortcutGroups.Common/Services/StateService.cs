using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TaskbarShortcutGroups.Common.Extensions;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Models.Serialization;

namespace TaskbarShortcutGroups.Common.Services;

public class StateService : IStateService
{
    // private static readonly string TaskbarLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar");
    private static readonly string ConfigLocation = Path.Join(Directory.GetCurrentDirectory(), "config");
    private static readonly string ShortcutsLocation = Path.Join(ConfigLocation, "shortcuts");
    private static readonly string IconsLocation = Path.Join(ConfigLocation, "icons");
    private static readonly string StateFileLocation = Path.Join(ConfigLocation, "ShortcutGroups.xml");
    private static readonly string ApplicationExecutable = Assembly.GetEntryAssembly()!.Location.Replace(".dll", ".exe");
    private readonly IFactory<Shortcut> shortcutFactory;
    private readonly IFactory<ShortcutGroup> shortcutGroupFactory;

    public StateService(IFactory<ShortcutGroup> shortcutGroupFactory, IFactory<Shortcut> shortcutFactory)
    {
        this.shortcutGroupFactory = shortcutGroupFactory ?? throw new ArgumentNullException(nameof(shortcutGroupFactory));
        this.shortcutFactory = shortcutFactory ?? throw new ArgumentNullException(nameof(shortcutFactory));
        ShortcutGroups = new List<ShortcutGroup>();
        LoadState();
    }

    public List<ShortcutGroup> ShortcutGroups { get; private set; }

    public void SaveState()
    {
        XmlSerializer serializer = new(typeof(ShortcutGroupDefinitions));
        Directory.CreateDirectory(ConfigLocation);
        var toSerialize = new ShortcutGroupDefinitions {ShortcutGroups = ShortcutGroups.ToList()};
        using var fileStream = new FileStream(StateFileLocation, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new XmlTextWriter(fileStream, Encoding.Default) {Formatting = Formatting.Indented};
        serializer.Serialize(writer, toSerialize);
        fileStream.Flush();
        fileStream.Close();
        foreach (var group in ShortcutGroups)
            CreateShortcutIfNotExist(group);
    }

    public void LoadState()
    {
        XmlSerializer serializer = new(typeof(ShortcutGroupDefinitions));
        if (!File.Exists(StateFileLocation))
            return;
        using var fileStream = new FileStream(StateFileLocation, FileMode.Open, FileAccess.Read);
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
        var shortcutPath = Path.Join(ShortcutsLocation, $"{group.Name}.lnk");
        var iconPath = Path.Join(IconsLocation, $"{group.Name}.ico");
        new Bitmap(group.IconPath).ToIcon().Save(iconPath, true);
        var shortcut = File.Exists(shortcutPath) 
            ? shortcutFactory.Construct(shortcutPath) 
            : shortcutFactory.Construct();
        shortcut.Name = group.Name;
        shortcut.ExecutablePath = ApplicationExecutable;
        shortcut.Arguments = group.Name;
        shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
        shortcut.IconLocation = new IconLocation(iconPath, 0);
        shortcut.Save(ShortcutsLocation);
    }
}