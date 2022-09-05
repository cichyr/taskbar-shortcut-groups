using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TaskbarShortcutGroups.Models.ComImports;

namespace TaskbarShortcutGroups.Models;

public class ShellLinkService
{
    private readonly IShellLink shellLink;

    public ShellLinkService()
    {
        shellLink = (IShellLink) new ShellLink();
    }

    public void Open(string path)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global - Justification: Using COM Imports by GUID, no way to avoid it.
        ((IPersistFile) shellLink).Load(path, 0);
        var sb = new StringBuilder(1024);
        var data = new WIN32_FIND_DATAW();
        shellLink.GetPath(sb, sb.Capacity, out data, 0);
        var x = sb.ToString();
    }
}