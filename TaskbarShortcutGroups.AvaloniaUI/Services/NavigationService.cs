using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;
using FileDialogFilter = TaskbarShortcutGroups.Common.Models.FileDialogFilter;

namespace TaskbarShortcutGroups.AvaloniaUI.Services;

internal class NavigationService : IAvaloniaNavigationService
{
    private readonly IDialogService dialogService;
    private string baseTitle = string.Empty;

    public NavigationService()
    {
        dialogService = new DialogService(this);
    }

    private Stack<ViewModelBase> NavigationStack { get; } = new();
    public Window CurrentWindow { get; private set; } = null!;

    public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        UpdateWindowTitle(viewModel);
        NavigationStack.Push((ViewModelBase)CurrentWindow.Content!);
        CurrentWindow.Content = viewModel;
    }

    public void NavigateBack()
    {
        var previousViewModel = NavigationStack.Pop();
        CurrentWindow.Title = !string.IsNullOrEmpty(previousViewModel.TitleNamePrefix)
            ? $"{previousViewModel.TitleNamePrefix} - {baseTitle}"
            : baseTitle;
        CurrentWindow.Content = previousViewModel;
    }

    public Task<string[]?> OpenFileDialog(string title, bool allowMultipleFiles, params FileDialogFilter[] filters)
        => dialogService.OpenFileDialog(title, allowMultipleFiles, filters);

    public void Setup(Window window)
    {
        CurrentWindow = window ?? throw new ArgumentNullException(nameof(window));
        baseTitle = window.Title!;
    }

    public void Navigate<TViewModel>(params object[] parameters) where TViewModel : ViewModelBase
    {
        var viewModel = CreateViewModelInstance<TViewModel>(parameters);
        UpdateWindowTitle(viewModel);
        NavigationStack.Push((ViewModelBase)CurrentWindow.Content!);
        CurrentWindow.Content = viewModel;
    }

    private void UpdateWindowTitle(ViewModelBase viewModel)
        => CurrentWindow.Title = string.IsNullOrEmpty(viewModel.TitleNamePrefix)
            ? baseTitle
            : $"{viewModel.TitleNamePrefix} - {baseTitle}";

    private static ViewModelBase CreateViewModelInstance<TViewModel>(object[] parameters)
        => Activator.CreateInstance(typeof(TViewModel), parameters) as ViewModelBase ??
           throw new InvalidOperationException("Failed to create view model.");
}