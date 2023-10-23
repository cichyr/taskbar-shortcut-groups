using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskbarShortcutGroups.Common.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    private readonly string titleNamePrefix = string.Empty;

    public string TitleNamePrefix
    {
        get => titleNamePrefix;
        protected init => SetProperty(ref titleNamePrefix, value);
    }
}