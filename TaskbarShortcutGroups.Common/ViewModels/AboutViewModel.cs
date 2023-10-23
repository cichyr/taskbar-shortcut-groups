using TaskbarShortcutGroups.Common.Models.Licensing;
using TaskbarShortcutGroups.Common.Providers;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private readonly ILicenseProvider licenseProvider;
    private readonly IVersionProvider versionProvider;

    public AboutViewModel(INavigationService navigationService, IStateService stateService, ILicenseProvider licenseProvider, IVersionProvider versionProvider)
        : base(navigationService, stateService)
    {
        this.licenseProvider = licenseProvider ?? throw new ArgumentNullException(nameof(licenseProvider));
        this.versionProvider = versionProvider ?? throw new ArgumentNullException(nameof(versionProvider));
        this.licenseProvider.Initialize();
    }

    public IEnumerable<ILicense> Licenses => licenseProvider.Licenses;

    public string VersionString => $"v{versionProvider.ProductVersion}";

    public void NavigateBack()
        => navigationService.NavigateBack();
}