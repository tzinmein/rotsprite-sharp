namespace RotSpriteSharp.Tests;

using RotSpriteSharp;

public class RotSpriteTests
{
    [Fact]
    public void Rotate_WhenAngleIs45Degrees_ReturnsCorrectDimensions()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        int width = 4;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 45.0);

        // Assert
        Assert.Equal(6, result.Width);
        Assert.Equal(6, result.Height);
        Assert.Equal(result.Pixels.Length, result.Width * result.Height);
    }

    [Fact]
    public void Rotate_WhenAngleIs90Or180Or270Degrees_UsesOptimizedPath()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5, 6];
        int width = 3;

        // Act
        var result90 = RotSprite.Rotate(buf, 0, width, 90.0);
        var result180 = RotSprite.Rotate(buf, 0, width, 180.0);
        var result270 = RotSprite.Rotate(buf, 0, width, 270.0);

        // Assert
        Assert.Equal(2, result90.Width);
        Assert.Equal(3, result90.Height);
        Assert.Equal([4, 1, 5, 2, 6, 3], result90.Pixels);

        Assert.Equal(3, result180.Width);
        Assert.Equal(2, result180.Height);
        Assert.Equal([6, 5, 4, 3, 2, 1], result180.Pixels);

        Assert.Equal(2, result270.Width);
        Assert.Equal(3, result270.Height);
        Assert.Equal([3, 6, 2, 5, 1, 4], result270.Pixels);
    }

    [Fact]
    public void Rotate_WhenAngleIs0Degrees_ReturnsOriginalImage()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5, 6];
        int width = 3;
        int height = 2;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 0.0);

        // Assert
        Assert.Equal(width, result.Width);
        Assert.Equal(height, result.Height);
        Assert.Equal(buf, result.Pixels);
    }

    [Fact]
    public void Rotate_WhenAngleIs360Degrees_ReturnsOriginalImage()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5, 6];
        int width = 3;
        int height = 2;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 360.0);

        // Assert
        Assert.Equal(width, result.Width);
        Assert.Equal(height, result.Height);
        Assert.Equal(buf, result.Pixels);
    }

    [Fact]
    public void Rotate_WhenBufferSizeDoesNotMatchWidth_ThrowsArgumentException()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5]; // Not divisible by width
        int width = 2;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RotSprite.Rotate(buf, 0, width, 45.0));
    }

    [Fact]
    public void Rotate_WhenWidthIsNegative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4];

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RotSprite.Rotate(buf, 0, -1, 45.0));
    }

    [Fact]
    public void Rotate_WhenBufferIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        int[] buf = [];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RotSprite.Rotate(buf, 0, 1, 45.0));
    }

    [Fact]
    public void Rotate_NonSquareImage_ReturnsCorrectDimensions()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        int width = 5;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 45.0);

        // Assert
        Assert.True(result.Width > 0);
        Assert.True(result.Height > 0);
        Assert.Equal(result.Pixels.Length, result.Width * result.Height);
    }

    [Fact]
    public void Rotate_LargeImage_DoesNotThrow()
    {
        // Arrange
        int width = 100;
        int height = 100;
        int[] buf = [.. Enumerable.Range(0, width * height)];

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 45.0);

        // Assert
        Assert.Equal(result.Pixels.Length, result.Width * result.Height);
    }

    [Fact]
    public void Rotate_AllPixelsSameValue_ReturnsImageWithSameValueOrEmptyColor()
    {
        // Arrange
        int[] buf = [.. Enumerable.Repeat(42, 16)];
        int width = 4;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 45.0);

        // Assert
        Assert.Contains(42, result.Pixels);
    }

    [Fact]
    public void Rotate_WithTransparentPixels_HandlesEmptyColor()
    {
        // Arrange
        int[] buf = [0, 0, 0, 0];
        int width = 2;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, 45.0);

        // Assert
        Assert.All(result.Pixels, p => Assert.Equal(0, p));
    }

    [Fact]
    public void Rotate_NullBuffer_ThrowsArgumentException()
    {
        // Arrange
        int[]? buf = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RotSprite.Rotate(buf!, 0, 2, 45.0));
    }

    [Fact]
    public void Rotate_ZeroWidth_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int[] buf = [1, 2, 3, 4];

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => RotSprite.Rotate(buf, 0, 0, 45.0));
    }

    [Theory]
    [InlineData(-45.0)]
    [InlineData(405.0)]
    public void Rotate_NegativeOrLargeAngle_NormalizesAngle(double angle)
    {
        // Arrange
        int[] buf = [1, 2, 3, 4];
        int width = 2;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, angle);

        // Assert
        Assert.Equal(result.Pixels.Length, result.Width * result.Height);
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(89.9999)]
    [InlineData(22.5)]
    [InlineData(123.4)]
    public void Rotate_NonIntegerAngles_ReturnsValidImage(double angle)
    {
        // Arrange
        int[] buf = [1, 2, 3, 4];
        int width = 2;

        // Act
        var result = RotSprite.Rotate(buf, 0, width, angle);

        // Assert
        Assert.Equal(result.Pixels.Length, result.Width * result.Height);
    }
}
