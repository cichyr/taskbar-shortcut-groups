using Windows.Win32;
using TaskbarShortcutGroups.Common.Models;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Windows.Services;

public class OsService : IOsService
{
    public PointerLocation GetCursorPosition()
    {
        PInvoke.GetCursorPos(out var point);
        return new PointerLocation(point.X, point.Y);
    }
}