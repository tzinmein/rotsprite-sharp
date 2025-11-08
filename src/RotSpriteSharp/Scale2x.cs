namespace RotSpriteSharp;

/// <summary>
/// Provides fast upscaling of pixel art sprites using the Scale2x algorithm.
/// </summary>
public static class Scale2x
{
    private const int ScaleFactor = 2;

    /// <summary>
    /// Scales a pixel buffer by 2x using the Scale2x algorithm (array overload).
    /// </summary>
    /// <typeparam name="T">Pixel type.</typeparam>
    /// <param name="buf">Source pixel buffer as an array.</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <returns>A <see cref="RotatedImage{T}"/> containing the scaled image.</returns>
    public static RotatedImage<T> Scale<T>(T[] buf, int width, int height)
        where T : notnull
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        if (buf.Length != width * height)
            throw new ArgumentException(
                "Buffer length does not match width * height.",
                nameof(buf)
            );

        int scaledWidth = width * ScaleFactor;
        int scaledHeight = height * ScaleFactor;
        var scaled = new T[scaledWidth * scaledHeight];
        for (int i = 0; i < scaled.Length; i++)
            scaled[i] = buf[0];

        // Center
        for (int y = 1; y < height - 1; y++)
        {
            int currentY = y * width;
            int scaledCurrentY = y * ScaleFactor * scaledWidth;
            for (int x = 1; x < width - 1; x++)
            {
                int pos = currentY + x;
                ApplyScale2xBlock(
                    scaled,
                    scaledCurrentY + (x * ScaleFactor),
                    scaledWidth,
                    buf[pos],
                    buf[pos - width],
                    buf[pos - 1],
                    buf[pos + width],
                    buf[pos + 1]
                );
            }

            int previousY = currentY - width;
            int nextY = currentY + width;
            // Left most column
            var p = buf[currentY];
            ApplyScale2xBlock(
                scaled,
                scaledCurrentY,
                scaledWidth,
                p,
                buf[previousY],
                p,
                buf[nextY],
                buf[currentY + 1]
            );
            // Right most column
            p = buf[nextY - 1];
            ApplyScale2xBlock(
                scaled,
                scaledCurrentY + scaledWidth - 2,
                scaledWidth,
                p,
                buf[currentY - 1],
                buf[nextY - 2],
                buf[nextY + width - 1],
                p
            );
        }

        // First and last rows
        for (int x = 1; x < width - 1; x++)
        {
            // First row
            var p = buf[x];
            ApplyScale2xBlock(
                scaled,
                x * ScaleFactor,
                scaledWidth,
                p,
                p,
                buf[x - 1],
                buf[x + width],
                buf[x + 1]
            );
            // Last row
            int pos = ((height - 1) * width) + x;
            p = buf[pos];
            int scaledCurrentY = (height - 1) * ScaleFactor * scaledWidth;
            ApplyScale2xBlock(
                scaled,
                scaledCurrentY + (x * ScaleFactor),
                scaledWidth,
                p,
                buf[pos - width],
                buf[pos - 1],
                p,
                buf[pos + 1]
            );
        }

        // Corners
        ApplyCorners(buf, scaled, width, height, scaledWidth, scaledHeight);

        return new RotatedImage<T>(scaledWidth, scaledHeight, scaled);
    }

    private static void ApplyCorners<T>(
        T[] buf,
        T[] scaled,
        int width,
        int height,
        int scaledWidth,
        int scaledHeight
    )
    {
        // Top left
        {
            var p = buf[0];
            ApplyScale2xBlock(scaled, 0, scaledWidth, p, p, p, buf[width], buf[1]);
        }
        // Top right
        {
            int rightX = width - 1;
            var p = buf[rightX];
            ApplyScale2xBlock(
                scaled,
                scaledWidth - 2,
                scaledWidth,
                p,
                p,
                buf[rightX - 1],
                buf[rightX + width],
                p
            );
        }
        // Bottom left
        {
            int bottomY = (height - 1) * width;
            var p = buf[bottomY];
            ApplyScale2xBlock(
                scaled,
                (scaledHeight - 2) * scaledWidth,
                scaledWidth,
                p,
                buf[bottomY - width],
                p,
                p,
                buf[bottomY + 1]
            );
        }
        // Bottom right
        {
            int rightX = width - 1;
            int bottomY = (height - 1) * width;
            int bottomRightPos = bottomY + rightX;
            var p = buf[bottomRightPos];
            ApplyScale2xBlock(
                scaled,
                ((scaledHeight - 2) * scaledWidth) + scaledWidth - 2,
                scaledWidth,
                p,
                buf[bottomRightPos - width],
                buf[bottomRightPos - 1],
                p,
                p
            );
        }
    }

    // Helper: apply the block on the buffer
    private static void ApplyScale2xBlock<T>(
        T[] scaled,
        int pos,
        int width,
        T center,
        T up,
        T left,
        T down,
        T right
    )
    {
        var (a, b, c, d) = CalculateScale2xBlock(center, up, left, down, right);
        scaled[pos] = a;
        scaled[pos + 1] = b;
        scaled[pos + width] = c;
        scaled[pos + width + 1] = d;
    }

    // Helper: convert a single pixel to an upscaled 2x2 block
    private static (T, T, T, T) CalculateScale2xBlock<T>(T center, T up, T left, T down, T right)
    {
        var eq = EqualityComparer<T>.Default;
        T a =
            (eq.Equals(left, up) && !eq.Equals(left, down) && !eq.Equals(up, right)) ? up : center;
        T b =
            (eq.Equals(up, right) && !eq.Equals(up, left) && !eq.Equals(right, down))
                ? right
                : center;
        T c =
            (eq.Equals(down, left) && !eq.Equals(down, right) && !eq.Equals(left, up))
                ? left
                : center;
        T d =
            (eq.Equals(right, down) && !eq.Equals(right, up) && !eq.Equals(down, left))
                ? down
                : center;
        return (a, b, c, d);
    }
}
