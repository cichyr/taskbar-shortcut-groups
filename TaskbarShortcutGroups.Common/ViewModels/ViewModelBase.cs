using CommunityToolkit.Mvvm.ComponentModel;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected readonly INavigationService navigationService;
    protected readonly IStateService stateService;
    private readonly string titleNamePrefix = string.Empty;

    protected ViewModelBase(INavigationService navigationService, IStateService stateService)
    {
        this.navigationService = navigationService;
        this.stateService = stateService;
    }

    public string TitleNamePrefix
    {
        get => titleNamePrefix;
        protected init => SetProperty(ref titleNamePrefix, value);
    }

    public void NavigateBack()
        => navigationService.NavigateBack();
}