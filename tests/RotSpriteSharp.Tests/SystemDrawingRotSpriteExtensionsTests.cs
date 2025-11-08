using System.Drawing;
using System.Runtime.Versioning;
using RotSpriteSharp.SystemDrawingExtensions;

namespace RotSpriteSharp.Tests;

public class SystemDrawingRotSpriteExtensionsTests
{
    [SupportedOSPlatform("windows")]
    [Fact]
    public void RotateWithRotSprite_Bitmap_RotatesImage()
    {
        if (!OperatingSystem.IsWindows())
            return;
        var bitmap = new Bitmap(2, 2);
        bitmap.SetPixel(0, 0, Color.Red);
        bitmap.SetPixel(1, 0, Color.Green);
        bitmap.SetPixel(0, 1, Color.Blue);
        bitmap.SetPixel(1, 1, Color.Yellow);

        var rotated = bitmap.RotateWithRotSprite(90);
        Assert.Equal(2, rotated.Width);
        Assert.Equal(2, rotated.Height);
        Assert.IsType<Bitmap>(rotated);
    }

    [SupportedOSPlatform("windows")]
    [Fact]
    public void RotateWithRotSprite_ColorArray_RotatesPixels()
    {
        if (!OperatingSystem.IsWindows())
            return;
        var pixels = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
        var rotated = pixels.RotateWithRotSprite(2, 90);
        Assert.Equal(4, rotated.Length);
        Assert.IsType<Color[]>(rotated);
    }
}
