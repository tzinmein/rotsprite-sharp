[![Build & Publish NuGet](https://github.com/tzinmein/rotsprite-sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tzinmein/rotsprite-sharp/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/RotSpriteSharp.svg?logo=nuget)](https://www.nuget.org/packages/RotSpriteSharp)
[![License: AGPL-3.0-or-later](https://img.shields.io/badge/license-AGPL--3.0--or--later-blue.svg)](#license)

# RotSpriteSharp (.NET port)

This is a port of the [rotsprite-rs](https://github.com/tversteeg/rotsprite) Rust library, which implements the [RotSprite algorithm](https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#RotSprite) for high-quality pixel art rotation. The port aims to faithfully reproduce the original algorithm and provide a demo for batch rotation and comparison. It supports all currently supported .NET LTS versions.

**RotSpriteSharp is a stable library. Future updates will mainly address dependency upgrades, support for new .NET LTS releases, and occasional new extensions. The core algorithm is complete and production-ready.**

## .NET LTS Support Timetable

| Version | Release Date | End of Support |
|---------|--------------|----------------|
| .NET 8  | Nov 2023     | Nov 2026       |
| .NET 10 | Nov 2025     | Nov 2028       |

For more details, see the official [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core).

## About RotSpriteSharp

RotSpriteSharp is a pixel art rotation algorithm that preserves sharpness and detail, outperforming standard aliased rotation methods for sprites and pixel art. It works by:

1. **Pixel-Guessing Enlargement**: Scales the input image up by 8x using a custom algorithm inspired by Scale2x, checking for pixel similarity rather than equality.
2. **Optimal Rotation Offset**: Searches for the best rotation offset by iterating over a small grid, rotating the enlarged image, and choosing the offset with minimal color difference.
3. **Aliased Rotation & Downscaling**: Rotates and scales the image back to the original size using nearest-neighbor methods.
4. **Single-Pixel Detail Correction**: Optionally post-processes the output to fix overlooked single-pixel details.

This approach uses pixel-level heuristics to approximate high-quality rotation, resulting in visually pleasing, sharp, and smooth rotated sprites.

## Usage

You can run the demo project (`RotCon`) with command-line parameters:

```
RotCon.exe <inputPath> [outputDir] [angles]
```

- `inputPath`: Path to the input image (default: `threeforms.png`)
- `outputDir`: Optional output directory for results (default: current directory)
- `angles`: Optional comma-separated list of rotation angles (default: `0,45,90,135,180,225,270,315`)

Example:

```
RotCon.exe threeforms.png out 0,90,180,270
```

This will output rotated images using the RotSpriteSharp algorithm, saving them in the `out` directory as `threeforms_rot0.png`, `threeforms_rot90.png`, etc.

### Library API Example

The core rotation API is provided by the `RotSpriteSharp` class:

```csharp
using RotSpriteSharp;
using SixLabors.ImageSharp.PixelFormats;

// Load your pixel buffer (e.g., from ImageSharp)
Rgba32[] pixels = ...;
int width = ...;
double angle = 45.0;
Rgba32 emptyColor = new Rgba32(0, 0, 0, 0);

// Rotate the image
var rotated = RotSpriteSharp.Rotate(pixels, emptyColor, width, angle);

// rotated.Pixels, rotated.Width, rotated.Height
```

- For angles divisible by 90, the algorithm uses a fast path (no upscaling).
- For other angles, the image is upscaled by 8x, rotated, and downscaled for high-quality results.

## Extension Projects & Usage

RotSpriteSharp provides extension libraries for popular graphics APIs:

### SkiaSharp Extensions

Add a reference to `RotSpriteSharp.SkiaSharpExtensions` and use:

```csharp
using RotSpriteSharp.SkiaSharpExtensions;
using SkiaSharp;

// Rotate a SKBitmap
SKBitmap bitmap = ...;
SKBitmap rotated = bitmap.RotateWithRotSprite(45);

// Rotate a SKColor[] pixel buffer
SKColor[] pixels = ...;
int width = ...;
SKColor[] rotatedPixels = pixels.RotateWithRotSprite(width, 45);
```

### System.Drawing Extensions

Add a reference to `RotSpriteSharp.SystemDrawingExtensions` and use (Windows only):

```csharp
using RotSpriteSharp.SystemDrawingExtensions;
using System.Drawing;

// Rotate a Bitmap
Bitmap bitmap = ...;
Bitmap rotated = bitmap.RotateWithRotSprite(45);

// Rotate a Color[] pixel buffer
Color[] pixels = ...;
int width = ...;
Color[] rotatedPixels = pixels.RotateWithRotSprite(width, 45);
```

> **Note:** System.Drawing.Common is only supported on Windows. The extension methods will throw `PlatformNotSupportedException` on other platforms.

## Projects

- `RotSpriteSharp`: Core library implementing the algorithm (no external dependencies).
- `RotCon`: Console demo for batch rotation and comparison (uses SkiaSharp for image loading and saving).

## Requirements

- All currently supported .NET LTS versions
- [SkiaSharp](https://github.com/mono/SkiaSharp) (only required for the `RotCon` demo application)

## Attribution & Credits

- **Original RotSprite algorithm** by [Xenowhirl](https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#RotSprite)
- **Rust implementation** by [tversteeg/rotsprite](https://github.com/tversteeg/rotsprite)
- **Pixel Art** by [Redshrike](https://opengameart.org/content/3-form-rpg-boss-harlequin-epicycle)

This .NET port is based on the Rust implementation and strives to maintain algorithmic fidelity with the original RotSprite algorithm. For more details on the algorithm, see the [RotSprite Wikipedia page](https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#RotSprite) and the [Rust implementation](https://github.com/tversteeg/rotsprite).

## License

See LICENSE file for details. This project is licensed under the AGPL-3.0-or-later, following the original Rust implementation.

---
