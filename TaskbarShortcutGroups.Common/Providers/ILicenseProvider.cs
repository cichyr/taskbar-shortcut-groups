using TaskbarShortcutGroups.Common.Models.Licensing;

namespace TaskbarShortcutGroups.Common.Providers;

public interface ILicenseProvider
{
    IEnumerable<ILicense> Licenses { get; }
    void Initialize();
}