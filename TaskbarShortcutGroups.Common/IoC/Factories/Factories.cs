using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.Common.IoC.Factories;

public interface IShortcutViewModelFactory
{
    ShortcutViewModel Create(string path, IShortcutGroup parentGroup);
    ShortcutViewModel Create(IShortcut shortcut);
}

public class ShortcutViewModelFactory : IShortcutViewModelFactory
{
    private readonly IShortcutFactory shortcutFactory;

    public ShortcutViewModelFactory(IShortcutFactory shortcutFactory)
    {
        this.shortcutFactory = shortcutFactory;
    }
    
    public ShortcutViewModel Create(string path, IShortcutGroup parentGroup)
        => new(path, parentGroup, shortcutFactory);
    public ShortcutViewModel Create(IShortcut shortcut)
        => new(shortcut);
}

public interface IShortcutGroupEditorViewModelFactory
{
    ShortcutGroupEditorViewModel Create(IShortcutGroup shortcutGroup);
    ShortcutGroupEditorViewModel Create();
}

public class ShortcutGroupEditorViewModelFactory : IShortcutGroupEditorViewModelFactory
{
    private readonly INavigationService navigationService;
    private readonly IStateService stateService;
    private readonly IShortcutViewModelFactory shortcutViewModelFactory;

    public ShortcutGroupEditorViewModelFactory(INavigationService navigationService, IStateService stateService, IShortcutViewModelFactory shortcutViewModelFactory)
    {
        this.navigationService = navigationService;
        this.stateService = stateService;
        this.shortcutViewModelFactory = shortcutViewModelFactory;
    }

    public ShortcutGroupEditorViewModel Create(IShortcutGroup shortcutGroup)
    {
        return new ShortcutGroupEditorViewModel(navigationService, stateService, shortcutViewModelFactory, shortcutGroup);
    }

    public ShortcutGroupEditorViewModel Create()
    {
        return new ShortcutGroupEditorViewModel(navigationService, stateService, shortcutViewModelFactory);
    }
}

public interface IShortcutFactory
{
    IShortcut Create();
    IShortcut Create(string path);
}

public interface IShortcutGroupFactory
{
    IShortcutGroup Create();
}

public class ShortcutGroupFactory : IShortcutGroupFactory
{
    public IShortcutGroup Create()
    {
        return new ShortcutGroup();
    }
}