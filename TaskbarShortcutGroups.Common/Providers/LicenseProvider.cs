using TaskbarShortcutGroups.Common.Constants;
using TaskbarShortcutGroups.Common.Models.Licensing;

namespace TaskbarShortcutGroups.Common.Providers;

public class LicenseProvider : ILicenseProvider
{
    private bool initialized;
    private IEnumerable<ILicense>? licenses;

    public void Initialize()
    {
        if (initialized)
            return;

        var newLicenses = new List<ILicense>();
        initialized = true;
        var licenseFiles = Directory.GetFiles(StorageLocation.Licenses);
        foreach (var licenseFile in licenseFiles)
        {
            using var streamReader = File.OpenText(licenseFile);
            var componentName = streamReader.ReadLine();
            var author = streamReader.ReadLine();
            var text = streamReader.ReadToEnd();
            newLicenses.Add(new License(componentName!, author!, text.Trim()));
        }

        licenses = newLicenses;
    }

    public IEnumerable<ILicense> Licenses
    {
        get
        {
            if (!initialized)
                throw new InvalidOperationException("Provider must be initialized first!");
            return licenses!;
        }
    }
}