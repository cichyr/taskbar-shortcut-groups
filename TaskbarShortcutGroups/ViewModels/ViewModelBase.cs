using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;
using TaskbarShortcutGroups.Services;

namespace TaskbarShortcutGroups.ViewModels;

public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
{
    protected INavigationService navigationService = NavigationService.Instance;
    public string TitleNamePrefix { get; protected set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}