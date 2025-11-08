using System.Drawing;
using System.Runtime.Versioning;

namespace RotSpriteSharp.SystemDrawingExtensions;

/// <summary>
/// Extension methods for using RotSpriteSharp with System.Drawing bitmaps and pixel arrays.
/// </summary>
public static class SystemDrawingRotSpriteExtensions
{
    [SupportedOSPlatform("windows")]
    public static Bitmap RotateWithRotSprite(this Bitmap bitmap, int angle)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "System.Drawing.Common is only supported on Windows."
            );

        var width = bitmap.Width;
        var height = bitmap.Height;
        var pixels = new uint[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var c = bitmap.GetPixel(x, y);
                pixels[y * width + x] =
                    ((uint)c.A << 24) | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;
            }
        }
        var emptyColor = 0u;
        var rotated = RotSprite.Rotate(pixels, emptyColor, width, angle);
        var outBitmap = new Bitmap(rotated.Width, rotated.Height);
        for (int y = 0; y < rotated.Height; y++)
        {
            for (int x = 0; x < rotated.Width; x++)
            {
                int idx = y * rotated.Width + x;
                var val = rotated.Pixels[idx];
                var color = Color.FromArgb(
                    (byte)((val >> 24) & 0xFF),
                    (byte)((val >> 16) & 0xFF),
                    (byte)((val >> 8) & 0xFF),
                    (byte)(val & 0xFF)
                );
                outBitmap.SetPixel(x, y, color);
            }
        }
        return outBitmap;
    }

    /// <summary>
    /// Rotates a Color pixel array using RotSprite.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static Color[] RotateWithRotSprite(this Color[] pixels, int width, int angle)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("System.Drawing.Common is only supported on Windows.");

        var uintPixels = new uint[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            var c = pixels[i];
            uintPixels[i] = ((uint)c.A << 24) | ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;
        }
        var emptyColor = 0u;
        var rotated = RotSprite.Rotate(uintPixels, emptyColor, width, angle);
        var result = new Color[rotated.Pixels.Length];
        for (int i = 0; i < rotated.Pixels.Length; i++)
        {
            var val = rotated.Pixels[i];
            result[i] = Color.FromArgb(
                (byte)((val >> 24) & 0xFF),
                (byte)((val >> 16) & 0xFF),
                (byte)((val >> 8) & 0xFF),
                (byte)(val & 0xFF)
            );
        }
        return result;
    }
}
