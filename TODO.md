# RotSpriteSharp Library Refactor TODO

## API Improvements
- [x] Refactor all public methods to return record types or classes instead of tuples (e.g., `RotatedImage<T>`)
- [x] Update demo app and tests to use new return types and remove unnecessary wrappers (e.g., use int[] directly)
- [x] Plan and implement extension projects for popular graphics libraries:
    - [x] RotSpriteSharp.SkiaSharpExtensions
    - [x] RotSpriteSharp.SystemDrawingExtensions

## Performance & Usability
- [x] Add overloads for common pixel types (e.g., `SKColor`, `Color`) in extension projects

## Documentation
- [x] Ensure all public APIs have XML documentation
- [x] Add usage examples to README
- [x] Document extension projects and their usage

## Consistency & Modern C#
- [x] Use nullability annotations where appropriate
- [x] Use consistent naming conventions for methods and types
- [x] Provide async overloads for I/O or long-running operations if needed (reviewed, not required for current API)

## Testing
- [x] Update unit tests to use new return types and remove unnecessary wrappers
- [x] Add more tests for edge cases and error handling
- [x] Add tests for extension projects

---
This file tracks planned improvements for making the RotSpriteSharp library more idiomatic and user-friendly in modern C#.