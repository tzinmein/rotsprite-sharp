namespace RotSpriteSharp;

/// <summary>
/// Provides generic image rotation functionality for pixel buffers.
/// </summary>
public static class RotSprite
{
    private const int UpscaleFactor = 8;
    private const int ScaleSteps = 3; // 2x, 4x, 8x

    /// <summary>
    /// Rotates a pixel buffer by the specified angle using the RotSprite algorithm.
    /// </summary>
    /// <typeparam name="T">The pixel type.</typeparam>
    /// <param name="buf">The source pixel buffer. Must not be null.</param>
    /// <param name="emptyColor">The color to use for empty pixels.</param>
    /// <param name="width">The width of the source image.</param>
    /// <param name="rotation">The rotation angle in degrees.</param>
    /// <returns>A <see cref="RotatedImage{T}"/> containing the rotated image data.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer is empty or dimensions don't match.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width is not positive.</exception>
    public static RotatedImage<T> Rotate<T>(T[] buf, T emptyColor, int width, double rotation)
        where T : notnull
    {
        if (buf == null || buf.Length == 0)
            throw new ArgumentException("Buffer cannot be empty.", nameof(buf));
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        if (buf.Length % width != 0)
        {
            throw new ArgumentException(
                "Image size doesn't match with supplied width.",
                nameof(buf)
            );
        }

        int height = buf.Length / width;
        rotation = ((rotation % 360.0) + 360.0) % 360.0;

        // If there's no rotation, return the original buffer
        if (rotation == 0.0)
        {
            return new RotatedImage<T>(width, height, (T[])buf.Clone());
        }

        // Special case: rotation is divisible by 90, skip upscaling
        if (rotation % 90.0 == 0.0)
        {
            if (Math.Abs(rotation - 90.0) < double.Epsilon)
                return Rotate90(buf, width, height);
            if (Math.Abs(rotation - 180.0) < double.Epsilon)
                return Rotate180(buf, width, height);
            if (Math.Abs(rotation - 270.0) < double.Epsilon)
                return Rotate270(buf, width, height);
            return new RotatedImage<T>(width, height, (T[])buf.Clone());
        }

        // Upscale the image using the scale2x algorithm (2x, 4x, 8x)
        var scaled2x = Scale2x.Scale(buf, width, height);
        var scaled4x = Scale2x.Scale(scaled2x.Pixels, scaled2x.Width, scaled2x.Height);
        var scaled8x = Scale2x.Scale(scaled4x.Pixels, scaled4x.Width, scaled4x.Height);

        // Rotate the image
        return RotateImage(
            scaled8x.Pixels,
            emptyColor,
            scaled8x.Width,
            scaled8x.Height,
            rotation,
            UpscaleFactor
        );
    }

    /// <summary>
    /// Rotates a pixel buffer by the specified angle using the RotSprite algorithm.
    /// </summary>
    /// <typeparam name="T">The pixel type.</typeparam>
    /// <param name="buf">The source pixel buffer as a span. Must not be null.</param>
    /// <param name="emptyColor">The color to use for empty pixels.</param>
    /// <param name="width">The width of the source image.</param>
    /// <param name="rotation">The rotation angle in degrees.</param>
    /// <returns>A <see cref="RotatedImage{T}"/> containing the rotated image data.</returns>
    public static RotatedImage<T> Rotate<T>(Span<T> buf, T emptyColor, int width, double rotation)
        where T : notnull
    {
        return Rotate(buf.ToArray(), emptyColor, width, rotation);
    }

    private static RotatedImage<T> RotateImage<T>(
        T[] buf,
        T emptyColor,
        int width,
        int height,
        double rotation,
        int downScaleFactor
    )
        where T : notnull
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        if (buf.Length != width * height)
            throw new ArgumentException(
                "Buffer length does not match width * height.",
                nameof(buf)
            );

        double radians = rotation * Math.PI / 180.0;
        double sin = Math.Sin(radians);
        double cos = Math.Cos(radians);

        double centerX = (width - 1) / 2.0;
        double centerY = (height - 1) / 2.0;

        var (resultWidth, resultHeight, newCenterX, newCenterY) = CalculateRotatedDimensions(
            width,
            height,
            centerX,
            centerY,
            sin,
            cos
        );

        double floatScale = downScaleFactor;
        int resultBufferWidth = (int)Math.Ceiling(resultWidth / floatScale);
        int resultBufferHeight = (int)Math.Ceiling(resultHeight / floatScale);

        var rotated = CreateRotatedBuffer(emptyColor, resultBufferWidth, resultBufferHeight);

        Parallel.For(
            0,
            (int)resultHeight,
            y =>
            {
                for (int x = 0; x < (int)resultWidth; x++)
                {
                    double deltaX = x - newCenterX;
                    double deltaY = y - newCenterY;
                    double sourceX = (deltaX * cos) + (deltaY * sin) + centerX;
                    double sourceY = (-deltaX * sin) + (deltaY * cos) + centerY;

                    if (sourceX >= 0.0 && sourceX < width && sourceY >= 0.0 && sourceY < height)
                    {
                        int destPosX = (int)(x / floatScale);
                        int destPosY = (int)(y / floatScale);
                        rotated[(destPosY * resultBufferWidth) + destPosX] = buf[
                            ((int)sourceY * width) + (int)sourceX
                        ];
                    }
                }
            }
        );
        return new RotatedImage<T>(resultBufferWidth, resultBufferHeight, rotated);
    }

    private static (
        double width,
        double height,
        double centerX,
        double centerY
    ) CalculateRotatedDimensions(
        int width,
        int height,
        double centerX,
        double centerY,
        double sin,
        double cos
    )
    {
        var corners = new (double x, double y)[]
        {
            (-centerX, -centerY),
            (width - 1 - centerX, -centerY),
            (width - 1 - centerX, height - 1 - centerY),
            (-centerX, height - 1 - centerY),
        };

        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;

        foreach (var (x, y) in corners)
        {
            double rotatedX = (x * cos) - (y * sin);
            double rotatedY = (x * sin) + (y * cos);
            minX = Math.Min(minX, rotatedX);
            minY = Math.Min(minY, rotatedY);
            maxX = Math.Max(maxX, rotatedX);
            maxY = Math.Max(maxY, rotatedY);
        }

        double resultWidth = Math.Ceiling(maxX - minX + 1);
        double resultHeight = Math.Ceiling(maxY - minY + 1);
        double newCenterX = (resultWidth - 1) / 2.0;
        double newCenterY = (resultHeight - 1) / 2.0;

        return (resultWidth, resultHeight, newCenterX, newCenterY);
    }

    private static T[] CreateRotatedBuffer<T>(T emptyColor, int width, int height)
    {
        var buffer = new T[width * height];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = emptyColor;
        return buffer;
    }

    private static RotatedImage<T> Rotate90<T>(T[] buf, int width, int height)
        where T : notnull
    {
        var rotated = new T[buf.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                rotated[(x * height) + (height - y - 1)] = buf[(y * width) + x];
            }
        }
        return new RotatedImage<T>(height, width, rotated);
    }

    private static RotatedImage<T> Rotate180<T>(T[] buf, int width, int height)
        where T : notnull
    {
        var rotated = new T[buf.Length];
        for (int i = 0; i < buf.Length; i++)
            rotated[i] = buf[buf.Length - 1 - i];
        return new RotatedImage<T>(width, height, rotated);
    }

    private static RotatedImage<T> Rotate270<T>(T[] buf, int width, int height)
        where T : notnull
    {
        var rotated90 = Rotate90(buf, width, height);
        return Rotate180(rotated90.Pixels, rotated90.Width, rotated90.Height);
    }
}
