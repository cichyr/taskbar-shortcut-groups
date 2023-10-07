using TaskbarShortcutGroups.Common.Models.Licensing;

namespace TaskbarShortcutGroups.Common.Providers;

public interface ILicenseProvider
{
    void Initialize();
    IEnumerable<ILicense> Licenses { get; }
}