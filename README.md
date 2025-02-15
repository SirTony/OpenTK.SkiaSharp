# OpenTK.SkiaSharp

OpenGL + SkiaSharp integration for .NET

# Basic Example

```csharp
using SirTony.OpenTK.SkiaSharp;
using SkiaSharp;

// set up OpenTK settings
var gameWindowSettings = new GameWindowSettings();
var nativeWindowSettings = new NativeWindowSettings
{
    Title = "OpenTK + SkiaSharp",
    ClientSize = new( 1280, 720 ),
    Vsync = VSyncMode.On,
};

// create the window
using var window = new MyWindow( gameWindowSettings, nativeWindowSettings );

// you can use the render event instead inheriting the class
window.RenderFrame += ( e, canvas ) =>
{
    // canvas.Clear( SKColors.CornflowerBlue );
    
    // Draw something here
};

// run the main event loop
window.Run();

// we can inherit SKWindow and override the OnRenderFrame method to draw on the canvas
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
