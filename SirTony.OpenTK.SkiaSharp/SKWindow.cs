using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;

namespace SirTony.OpenTK.SkiaSharp;

/// <summary>
///     Creates a new OpenGL-backed window for rendering with Skia.
/// </summary>
/// <param name="gameWindowSettings">The game window settings that will be passed onto the base <see cref="GameWindow" />.</param>
/// <param name="nativeWindowSettings">
///     The native window settings that will be passed onto the base
///     <see cref="GameWindow" />.
/// </param>
// ReSharper disable once InconsistentNaming
public class SKWindow(
    GameWindowSettings   gameWindowSettings,
    NativeWindowSettings nativeWindowSettings
) : GameWindow( gameWindowSettings, nativeWindowSettings )
{
    private readonly NativeWindowSettings   _nativeWindowSettings = nativeWindowSettings;
    private          GRGlInterface?         _gl;
    private          GRContext?             _gr;
    private          SKSurface?             _surface;
    private          GRBackendRenderTarget? _target;

    private SKCanvas Canvas => this._surface!.Canvas;

    new public event Action<FrameEventArgs, SKCanvas>? RenderFrame;

    /// <inheritdoc />
    protected override void OnLoad()
    {
        this._gl = GRGlInterface.Create();
        this._gr = GRContext.CreateGl( this._gl );
        this.CreateSurface();
        base.OnLoad();
    }

    /// <inheritdoc />
    protected override void OnResize( ResizeEventArgs e )
    {
        this.CreateSurface();
        base.OnResize( e );
    }

    /// <inheritdoc cref="GameWindow.OnRenderFrame" />
    /// <param name="canvas">The Skia canvas used for rendering.</param>
    protected virtual void OnRenderFrame( FrameEventArgs e, SKCanvas canvas ) => this.RenderFrame?.Invoke( e, canvas );

    /// <inheritdoc />
    protected sealed override void OnRenderFrame( FrameEventArgs e )
    {
        this.OnRenderFrame( e, this.Canvas );
        this._surface?.Flush();
        this.SwapBuffers();
        base.OnRenderFrame( e );
    }

    private void CreateSurface()
    {
        this._target?.Dispose();
        this._surface?.Dispose();

        this._target = new(
            this.ClientSize.X,
            this.ClientSize.Y,
            this._nativeWindowSettings.NumberOfSamples,
            8,
            new( 0, (uint)SizedInternalFormat.Rgba8 )
        );
        this._surface = SKSurface.Create( this._gr, this._target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888 );
        if( this._surface is null ) throw new InvalidOperationException( "failed to create Skia surface" );
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if( disposing )
        {
            this._surface?.Dispose();
            this._target?.Dispose();
            this._gr?.Dispose();
            this._gl?.Dispose();
        }

        base.Dispose( disposing );
    }
}
