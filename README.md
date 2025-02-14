# OpenTK.SkiaSharp

OpenTK + SkiaSharp integration.

# Minimal Example

```csharp
using SirTony.OpenTK.SkiaSharp;

var gameWindowSettings = new GameWindowSettings();
var nativeWindowSettings = new NativeWindowSettings
{
    Title = "OpenTK + SkiaSharp",
    ClientSize = new( 1280, 720 ),
    Vsync = VSyncMode.On,
};

using var window = new SKWindow( gameWindowSettings, nativeWindowSettings );
window.Run();
```
