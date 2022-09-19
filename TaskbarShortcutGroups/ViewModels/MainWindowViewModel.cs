using System;
using System.Collections.ObjectModel;
using TaskbarShortcutGroups.Models;

namespace TaskbarShortcutGroups.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase displayedViewModel;

    public MainWindowViewModel()
    {
        // navigationService.Setup(this);
        navigationService.Navigate<ShortcutGroupListViewModel>();
    }

    public string Greeting
    {
        get => "Welcome to Taskbar Shortcut Groups!";
        set => throw new NotImplementedException();
    }

    public ViewModelBase DisplayedViewModel
    {
        get => displayedViewModel;
        set
        {
            if (Equals(value, displayedViewModel)) return;
            displayedViewModel = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ShortcutGroup> ShortcutGroups { get; set; }
}