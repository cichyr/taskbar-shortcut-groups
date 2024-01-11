using TaskbarShortcutGroups.Common.Models;

namespace TaskbarShortcutGroups.Common.Services;

public interface IOsService
{
    PointerLocation GetCursorPosition();
}