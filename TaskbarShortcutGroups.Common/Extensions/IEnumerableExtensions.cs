namespace TaskbarShortcutGroups.Common.Extensions;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
        {
            action.Invoke(element);
        }
    }
    
    public static void ForEach<T>(T[] enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
        {
            action.Invoke(element);
        }
    }
}