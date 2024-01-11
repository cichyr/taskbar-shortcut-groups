using System.Diagnostics.CodeAnalysis;
using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.Providers;

public class StateProvider : IStateProvider
{
    private readonly IStateStore stateStore;

    public StateProvider(IStateStore stateStore)
    {
        this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        Directory.CreateDirectory(StorageLocation.Config);
        Directory.CreateDirectory(StorageLocation.Shortcuts);
        Directory.CreateDirectory(StorageLocation.Icons);
        ShortcutGroups = stateStore.Load()?.ToList() ?? new List<IShortcutGroup>();
    }

    public List<IShortcutGroup> ShortcutGroups { get; }

    public void Save()
        => stateStore.Save(ShortcutGroups);
}