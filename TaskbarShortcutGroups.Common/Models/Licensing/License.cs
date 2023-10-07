namespace TaskbarShortcutGroups.Common.Models.Licensing;

public class License : ILicense
{
    public License(string componentName, string author, string text)
    {
        ComponentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public string ComponentName { get; }

    public string Author { get; }

    public string Text { get; }
}