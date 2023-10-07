using TaskbarShortcutGroups.Common.Models.Licensing;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private readonly ILicenseProvider licenseProvider;

    public AboutViewModel(INavigationService navigationService, IStateService stateService, ILicenseProvider licenseProvider)
        : base(navigationService, stateService)
    {
        this.licenseProvider = licenseProvider;
        this.licenseProvider.Initialize();
    }

    public IEnumerable<ILicense> Licenses => licenseProvider.Licenses;

    public void Return()
        => navigationService.NavigateBack();
}