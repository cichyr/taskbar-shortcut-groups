using System;
using Avalonia.Controls;
using TaskbarShortcutGroups.ViewModels;

namespace TaskbarShortcutGroups.Services;

internal class NavigationService : INavigationService
{
#pragma warning disable CS8618 // It is initialized during application startup 
    private Window window;
#pragma warning restore CS8618
    private string baseTitle = string.Empty; 
    public static INavigationService Instance { get; } = new NavigationService();

    public void Navigate<TViewModel>(params object?[]? parameters) where TViewModel : ViewModelBase
    {
        var viewModel = CreateViewModelInstance<TViewModel>(parameters);
        if (!string.IsNullOrEmpty(viewModel.TitleNamePrefix))
            window.Title = $"{viewModel.TitleNamePrefix} - {baseTitle}";
        window.Content = viewModel;
    }
    
    private static ViewModelBase CreateViewModelInstance<TViewModel>(object?[]? parameters = null)
    {
        return Activator.CreateInstance(typeof(TViewModel), parameters) as ViewModelBase ??
               throw new InvalidOperationException("Failed to create view model.");
    }

    public void Setup(Window window)
    {
        this.window = window ?? throw new ArgumentNullException(nameof(window));
        this.baseTitle = window.Title;
    }
}