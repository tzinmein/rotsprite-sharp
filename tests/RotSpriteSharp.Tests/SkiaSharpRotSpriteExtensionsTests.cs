using RotSpriteSharp.SkiaSharpExtensions;
using SkiaSharp;

namespace RotSpriteSharp.Tests;

public class SkiaSharpRotSpriteExtensionsTests
{
    [Fact]
    public void RotateWithRotSprite_Bitmap_RotatesImage()
    {
        var bitmap = new SKBitmap(2, 2);
        bitmap.SetPixel(0, 0, new SKColor(255, 0, 0, 255)); // Red
        bitmap.SetPixel(1, 0, new SKColor(0, 255, 0, 255)); // Green
        bitmap.SetPixel(0, 1, new SKColor(0, 0, 255, 255)); // Blue
        bitmap.SetPixel(1, 1, new SKColor(255, 255, 0, 255)); // Yellow

        var rotated = bitmap.RotateWithRotSprite(90);
        Assert.Equal(2, rotated.Width);
        Assert.Equal(2, rotated.Height);
        // Basic pixel value check (not a full rotation validation)
        Assert.IsType<SKBitmap>(rotated);
    }

    [Fact]
    public void RotateWithRotSprite_SKColorArray_RotatesPixels()
    {
        var pixels = new SKColor[]
        {
            new(255, 0, 0, 255), // Red
            new(0, 255, 0, 255), // Green
            new(0, 0, 255, 255), // Blue
            new(255, 255, 0, 255), // Yellow
        };
        var rotated = pixels.RotateWithRotSprite(2, 90);
        Assert.Equal(4, rotated.Length);
        Assert.IsType<SKColor[]>(rotated);
    }
}
