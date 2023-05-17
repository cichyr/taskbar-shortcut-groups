using System.Threading.Tasks;
using TaskbarShortcutGroups.Common.IoC;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;
using TaskbarShortcutGroups.Common.ViewModels;

namespace TaskbarShortcutGroups.WinUI.Services;

public class NavigationService : INavigationService
{
    private readonly IStateService stateService;
    private readonly ShortcutGroupListViewModel shortcutGroupListViewModel;

    public NavigationService(IStateService stateService, IFactory<ShortcutGroupListViewModel> groupListFactory)
    {
        this.stateService = stateService;
        shortcutGroupListViewModel = groupListFactory.Construct();
    }

    /// <inheritdoc />
    public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        => throw new System.NotImplementedException();

    /// <inheritdoc />
    public void NavigateBack()
        => throw new System.NotImplementedException();

    /// <inheritdoc />
    public Task<string[]?> OpenFileDialog(string title, bool allowMultipleFiles, params FileDialogFilter[] filters)
        => throw new System.NotImplementedException();
}