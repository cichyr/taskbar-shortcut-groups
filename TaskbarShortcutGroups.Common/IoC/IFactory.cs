namespace TaskbarShortcutGroups.Common.IoC;

public interface IFactory<T>
{
    T Construct();
    T Construct(params object[] parameters);
}