namespace RotSpriteSharp;

/// <summary>
/// Represents a rotated image buffer with validated dimensions.
/// </summary>
/// <typeparam name="T">The pixel type.</typeparam>
public record RotatedImage<T> where T : notnull
{
    /// <summary>
    /// Gets the width of the image in pixels.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// Gets the height of the image in pixels.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// Gets the pixel data buffer.
    /// </summary>
    public T[] Pixels { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RotatedImage{T}"/> class with validation.
    /// </summary>
    /// <param name="width">The width of the image. Must be positive.</param>
    /// <param name="height">The height of the image. Must be positive.</param>
    /// <param name="pixels">The pixel data buffer. Must not be null and must match dimensions.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is not positive.</exception>
    /// <exception cref="ArgumentNullException">Thrown when pixels is null.</exception>
    /// <exception cref="ArgumentException">Thrown when pixels length doesn't match dimensions.</exception>
    public RotatedImage(int width, int height, T[] pixels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentNullException.ThrowIfNull(pixels);
        if (pixels.Length != width * height)
            throw new ArgumentException("Pixels length must equal Width * Height.", nameof(pixels));

        Width = width;
        Height = height;
        Pixels = pixels;
    }
}
