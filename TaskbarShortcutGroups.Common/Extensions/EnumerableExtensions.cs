namespace TaskbarShortcutGroups.Common.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<TElement>(this IEnumerable<TElement> elements, Action<TElement> action)
    {
        foreach (var element in elements)
        {
            action(element);
        }
    }
}