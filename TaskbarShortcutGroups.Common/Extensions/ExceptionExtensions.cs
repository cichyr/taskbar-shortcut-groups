using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TaskbarShortcutGroups.Common.Extensions;

public static class ExceptionExtensions
{
    public static void ThrowIfNull<T>([NotNull] T? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
    }
}