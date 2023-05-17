using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace TaskbarShortcutGroups.Avalonia.Converters;

public class IconToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Icon imageIcon)
        {
            return value is null
                ? new BindingNotification(new InvalidCastException("Expected icon object, received null"), BindingErrorType.Error)
                : new BindingNotification(new InvalidCastException($"Expected icon object, received {value.GetType().FullName}"), BindingErrorType.Error);
        }

        var bitmapTmp = imageIcon.ToBitmap();
        var bitmapData = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var bitmap1 = new Bitmap(global::Avalonia.Platform.PixelFormat.Bgra8888,
            AlphaFormat.Premul,
            bitmapData.Scan0,
            new PixelSize(bitmapData.Width, bitmapData.Height),
            new Vector(96, 96),
            bitmapData.Stride);
        bitmapTmp.UnlockBits(bitmapData);
        bitmapTmp.Dispose();
        return bitmap1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}