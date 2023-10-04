using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;
using FileDialogFilter = TaskbarShortcutGroups.Common.Models.FileDialogFilter;

namespace TaskbarShortcutGroups.Avalonia.Services;

internal class NavigationService : IAvaloniaNavigationService
{
    private readonly IDialogService dialogService;
    private string baseTitle = string.Empty;

    public NavigationService()
    {
        dialogService = new DialogService(this);
    }
    
    private Stack<object> NavigationStack { get; } = new();
    public Window CurrentWindow { get; private set; } = null!;

    public void Navigate<TViewModel>(params object[] parameters) where TViewModel : ViewModelBase
    {
        var viewModel = CreateViewModelInstance<TViewModel>(parameters);
        UpdateWindowTitle(viewModel);
        NavigationStack.Push(CurrentWindow.Content!);
        CurrentWindow.Content = viewModel;
    }

    public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        UpdateWindowTitle(viewModel);
        NavigationStack.Push(CurrentWindow.Content!);
        CurrentWindow.Content = viewModel;
    }

    private void UpdateWindowTitle(ViewModelBase viewModel)
        => CurrentWindow.Title = string.IsNullOrEmpty(viewModel.TitleNamePrefix)
            ? baseTitle
            : $"{viewModel.TitleNamePrefix} - {baseTitle}";

    public void NavigateBack()
    {
        var previousView = NavigationStack.Pop();
        if (previousView is ViewModelBase viewModel && !string.IsNullOrEmpty(viewModel.TitleNamePrefix))
            CurrentWindow.Title = $"{viewModel.TitleNamePrefix} - {baseTitle}";
        CurrentWindow.Content = previousView;
    }

    public Task<string[]?> OpenFileDialog(string title, bool allowMultipleFiles, params FileDialogFilter[] filters) 
        => dialogService.OpenFileDialog(title, allowMultipleFiles, filters);

    public void Setup(Window window)
    {
        CurrentWindow = window ?? throw new ArgumentNullException(nameof(window));
        baseTitle = window.Title!;
    }

    private static ViewModelBase CreateViewModelInstance<TViewModel>(object[] parameters)
    {
        return Activator.CreateInstance(typeof(TViewModel), parameters) as ViewModelBase ??
               throw new InvalidOperationException("Failed to create view model.");
    }
}