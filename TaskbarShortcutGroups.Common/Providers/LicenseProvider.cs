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
            var newLicense = new License
            {
                ComponentName = streamReader.ReadLine()!,
                Author = streamReader.ReadLine()!,
                Text = streamReader.ReadToEnd(),
            };
            newLicenses.Add(newLicense);
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