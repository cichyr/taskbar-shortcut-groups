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

public class SystemBitmapToAvaloniaBitmapConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Bitmap)
            return value;
        if (value is not System.Drawing.Bitmap systemBitmap)
            return value is null
                ? new BindingNotification(new InvalidCastException("Expected System.Drawing.Bitmap object, received null"), BindingErrorType.Error)
                : new BindingNotification(new InvalidCastException($"Expected System.Drawing.Bitmap object, received {value.GetType().FullName}"), BindingErrorType.Error);

        var bitmapData = systemBitmap.LockBits(new Rectangle(0, 0, systemBitmap.Width, systemBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var avaloniaBitmap = new Bitmap(global::Avalonia.Platform.PixelFormat.Bgra8888,
            AlphaFormat.Premul,
            bitmapData.Scan0,
            new PixelSize(bitmapData.Width, bitmapData.Height),
            new Vector(96, 96),
            bitmapData.Stride);
        systemBitmap.UnlockBits(bitmapData);
        systemBitmap.Dispose();
        return avaloniaBitmap;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}