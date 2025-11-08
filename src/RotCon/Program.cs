using System.CommandLine;
using RotSpriteSharp.SkiaSharpExtensions;
using SkiaSharp;

namespace RotCon;

/// <summary>
/// Console demo for rotating an image using the RotSprite algorithm.
/// </summary>
static class Program
{
    static int Main(string[] args)
    {
        var inputOption = new Option<string>("--input")
        {
            Description = "Input image path",
            DefaultValueFactory = _ => "threeforms.png",
        };
        var outputOption = new Option<string>("--output")
        {
            Description = "Output directory",
            DefaultValueFactory = _ => "",
        };
        var anglesOption = new Option<string>("--angles")
        {
            Description = "Comma-separated angles",
            DefaultValueFactory = _ => "0,45,90,135,180,225,270,315",
        };

        var rootCommand = new RootCommand("RotSprite demo")
        {
            inputOption,
            outputOption,
            anglesOption,
        };

        rootCommand.SetAction(parseResult =>
        {
            var inputPath = parseResult.GetValue(inputOption);
            var outputDir = parseResult.GetValue(outputOption);
            var anglesArg = parseResult.GetValue(anglesOption);

            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"File not found: {inputPath}");
                return 1;
            }

            var angles = (anglesArg ?? string.Empty)
                .Split(',')
                .Select(s => int.TryParse(s, out var a) ? a : (int?)null)
                .Where(a => a.HasValue)
                .Select(a => a!.Value);
            if (!angles.Any())
            {
                Console.WriteLine($"No valid angles specified. Example: 0,45,90");
                return 1;
            }

            using var bitmap = SKBitmap.Decode(inputPath);

            foreach (var angle in angles)
            {
                var outBitmap = bitmap.RotateWithRotSprite(angle);
                var outFile = $"{Path.GetFileNameWithoutExtension(inputPath)}_rot{angle}.png";
                if (!string.IsNullOrWhiteSpace(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                    outFile = Path.Combine(outputDir, outFile);
                }
                using var image = SKImage.FromBitmap(outBitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = File.OpenWrite(outFile);
                data.SaveTo(stream);
                Console.WriteLine($"Saved: {outFile}");
            }
            return 0;
        });

        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
