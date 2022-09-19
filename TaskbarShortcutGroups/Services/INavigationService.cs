using Avalonia.Controls;
using TaskbarShortcutGroups.ViewModels;

namespace TaskbarShortcutGroups.Services;

public interface INavigationService
{
    void Navigate<TViewModel>(params object[] parameters) where TViewModel : ViewModelBase;
    void Setup(Window window);
}