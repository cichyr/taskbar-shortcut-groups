using System.Drawing;
using System.Drawing.Imaging;

namespace TaskbarShortcutGroups.Common.Extensions;

public static class IconExtensions
{
    /// <summary>
    /// Saves icon to file in given path.
    /// </summary>
    /// <param name="icon"> The icon to save. </param>
    /// <param name="pathToSave"> The path of target file. </param>
    /// <param name="overwriteIfExists"> Whether the icon should be overwritten. </param>
    /// <returns> The given <paramref name="icon"/>. </returns>
    /// <exception cref="InvalidOperationException"> When <paramref name="pathToSave" /> exists and <paramref name="overwriteIfExists" /> is false. </exception>
    public static Icon Save(this Icon icon, string pathToSave, bool overwriteIfExists = false)
    {
        if (!Path.HasExtension(pathToSave) || Path.GetExtension(pathToSave) != ".ico")
            pathToSave = Path.ChangeExtension(pathToSave, ".ico");
        if (File.Exists(pathToSave))
        {
            if (overwriteIfExists)
                File.Delete(pathToSave);
            else
                throw new InvalidOperationException($"Cannot save icon, because file '{pathToSave}' exists!");
        }
        else if (!Directory.Exists(Path.GetDirectoryName(pathToSave)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pathToSave)!);
        }

        using var fileStream = new FileStream(pathToSave, FileMode.Create, FileAccess.Write);
        icon.Save(fileStream);
        return icon;
    }

    /// <summary>
    /// Converts any bitmap to icon.
    /// </summary>
    /// <param name="sourceBitmap"> The bitmap to convert. </param>
    /// <returns> The converted icon. </returns>
    public static Icon ToIcon(this Bitmap sourceBitmap)
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream);

        // Header
        binaryWriter.Write((short)0); // 0 : reserved
        binaryWriter.Write((short)1); // 2 : 1=ico, 2=cur
        binaryWriter.Write((short)1); // 4 : number of images

        // Image directory
        var w = sourceBitmap.Width;
        if (w >= 256)
            w = 0;
        binaryWriter.Write((byte)w); // 0 : width of image
        var h = sourceBitmap.Height;
        if (h >= 256)
            h = 0;
        binaryWriter.Write((byte)h); // 1 : height of image
        binaryWriter.Write((byte)0); // 2 : number of colors in palette
        binaryWriter.Write((byte)0); // 3 : reserved
        binaryWriter.Write((short)0); // 4 : number of color planes
        binaryWriter.Write((short)0); // 6 : bits per pixel
        var sizeHere = memoryStream.Position;
        binaryWriter.Write(0); // 8 : image size
        var start = (int)memoryStream.Position + 4;
        binaryWriter.Write(start); // 12: offset of image data

        // Image data
        sourceBitmap.Save(memoryStream, ImageFormat.Png);
        var imageSize = (int)memoryStream.Position - start;
        memoryStream.Seek(sizeHere, SeekOrigin.Begin);
        binaryWriter.Write(imageSize);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // And load it
        return new Icon(memoryStream);
    }
}