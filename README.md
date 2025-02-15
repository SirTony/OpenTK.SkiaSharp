# OpenTK.SkiaSharp

OpenGL + SkiaSharp integration for .NET

# Basic Example

```csharp
using SirTony.OpenTK.SkiaSharp;
using SkiaSharp;

var gameWindowSettings = new GameWindowSettings();
var nativeWindowSettings = new NativeWindowSettings
{
    Title = "OpenTK + SkiaSharp",
    ClientSize = new( 1280, 720 ),
    Vsync = VSyncMode.On,
};

using var window = new MyWindow( gameWindowSettings, nativeWindowSettings );
window.Run();

internal sealed class MyWindow : SKWindow
{
    public MyWindow( GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings )
        : base( gameWindowSettings, nativeWindowSettings )
    { }

    protected override void OnRenderFrame( FrameEventArgs e, SKCanvas canvas )
    {
        canvas.Clear( SKColors.CornflowerBlue );
        
        // Draw something here
        
        base.OnRenderFrame( e, canvas );
    }
}
```
