using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace TaskbarShortcutGroups.AvaloniaUI.Converters;

public class PathToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string imagePath)
            return string.IsNullOrEmpty(imagePath)
                ? new BindingNotification(default(Bitmap))
                : new Bitmap(imagePath);
        return value is null
            ? new BindingNotification(new InvalidCastException("Expected path string, received null"), BindingErrorType.Error)
            : new BindingNotification(new InvalidCastException($"Expected path string, received {value.GetType().FullName}"), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}