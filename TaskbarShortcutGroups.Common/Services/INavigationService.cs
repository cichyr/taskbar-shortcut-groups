using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.Common.Services;

/// <summary>
/// The service that provides basic navigation facilities.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to given view model instance.
    /// </summary>
    /// <param name="viewModel"> The view model to display. </param>
    /// <typeparam name="TViewModel"> The type of the view model. </typeparam>
    void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;

    /// <summary>
    /// Navigates back in the navigation tree.
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Opens the select file dialog.
    /// </summary>
    /// <param name="title"> The title of the dialog. </param>
    /// <param name="allowMultipleFiles"> A value indicating whether selection of multiple files should be possible. </param>
    /// <param name="filters"> The file dialog extension filters. </param>
    /// <returns> The paths to the files that were selected by the user. </returns>
    Task<string[]?> OpenFileDialog(string title, bool allowMultipleFiles, params FileDialogFilter[] filters);
}