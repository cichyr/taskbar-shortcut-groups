using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
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
        protected init
        {
            titleNamePrefix = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NavigateBack()
    {
        navigationService.NavigateBack();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}