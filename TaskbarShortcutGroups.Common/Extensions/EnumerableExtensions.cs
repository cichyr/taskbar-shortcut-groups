namespace TaskbarShortcutGroups.Common.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<TElement>(this IEnumerable<TElement> elements, Action<TElement> action)
    {
        ArgumentNullException.ThrowIfNull(elements);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var element in elements) action(element);
    }

    public static IEnumerable<TElement> WhereNotNull<TElement>(this IEnumerable<TElement?> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        return elements
            .Where(x => x != null)
            .Cast<TElement>();
    }
}