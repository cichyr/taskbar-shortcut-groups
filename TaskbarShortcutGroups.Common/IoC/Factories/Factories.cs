using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.Common.IoC.Factories;

public interface IShortcutViewModelFactory
{
    ShortcutViewModel Create(IShortcut shortcut);
}

public class ShortcutViewModelFactory : IShortcutViewModelFactory
{
    private readonly INavigationService navigationService;
    private readonly IStateService stateService;

    public ShortcutViewModelFactory(INavigationService navigationService, IStateService stateService)
    {
        this.navigationService = navigationService;
        this.stateService = stateService;
    }

    public ShortcutViewModel Create(IShortcut shortcut)
    {
        return new ShortcutViewModel(navigationService, stateService, shortcut);
    }
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

public class ShortcutFactory : IShortcutFactory
{
    public IShortcut Create()
    {
        return new Shortcut();
    }

    public IShortcut Create(string path)
    {
        return new Shortcut(path);
    }
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