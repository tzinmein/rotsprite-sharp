using SkiaSharp;

namespace RotSpriteSharp.SkiaSharpExtensions;

/// <summary>
/// Extension methods for using RotSpriteSharp with SkiaSharp bitmaps and pixel arrays.
/// </summary>
public static class SkiaSharpRotSpriteExtensions
{
    public static SKBitmap RotateWithRotSprite(this SKBitmap bitmap, int angle)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;
        var pixels = new uint[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var c = bitmap.GetPixel(x, y);
                pixels[y * width + x] =
                    ((uint)c.Alpha << 24) | ((uint)c.Red << 16) | ((uint)c.Green << 8) | c.Blue;
            }
        }
        var emptyColor = 0u;
        var rotated = RotSprite.Rotate(pixels, emptyColor, width, angle);
        var outBitmap = new SKBitmap(rotated.Width, rotated.Height);
        for (int y = 0; y < rotated.Height; y++)
        {
            for (int x = 0; x < rotated.Width; x++)
            {
                int idx = y * rotated.Width + x;
                var val = rotated.Pixels[idx];
                var color = new SKColor(
                    (byte)((val >> 16) & 0xFF),
                    (byte)((val >> 8) & 0xFF),
                    (byte)(val & 0xFF),
                    (byte)((val >> 24) & 0xFF)
                );
                outBitmap.SetPixel(x, y, color);
            }
        }
        return outBitmap;
    }

    /// <summary>
    /// Rotates a SKColor pixel array using RotSprite.
    /// </summary>
    public static SKColor[] RotateWithRotSprite(this SKColor[] pixels, int width, int angle)
    {
        var uintPixels = new uint[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            var c = pixels[i];
            uintPixels[i] = ((uint)c.Alpha << 24) | ((uint)c.Red << 16) | ((uint)c.Green << 8) | c.Blue;
        }
        var emptyColor = 0u;
        var rotated = RotSprite.Rotate(uintPixels, emptyColor, width, angle);
        var result = new SKColor[rotated.Pixels.Length];
        for (int i = 0; i < rotated.Pixels.Length; i++)
        {
            var val = rotated.Pixels[i];
            result[i] = new SKColor(
                (byte)((val >> 16) & 0xFF),
                (byte)((val >> 8) & 0xFF),
                (byte)(val & 0xFF),
                (byte)((val >> 24) & 0xFF)
            );
        }
        return result;
    }
}
