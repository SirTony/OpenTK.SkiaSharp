using SirTony.OpenTK.SkiaSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using Topten.RichTextKit;

using var window = new DemoWindow();
window.Run();

internal sealed class DemoWindow() : SKWindow(
    new(),
    new()
    {
        Title      = "Skia OpenGL Demo",
        ClientSize = new( 800, 800 ),
        Vsync      = VSyncMode.On,
    }
)
{
    private readonly SKFont _font = SKTypeface.Default.ToFont( 24 );

    private readonly SKPaint _paint = new()
    {
        Style       = SKPaintStyle.Fill,
        IsAntialias = true,
    };

    private readonly double[]  _samples   = new double[512];
    private readonly TextBlock _textBlock = new();

    private readonly TextPaintOptions _textPaint = new()
    {
        Hinting             = SKFontHinting.Full,
        SubpixelPositioning = true,
        Edging              = SKFontEdging.SubpixelAntialias,
    };

    private int           _index;
    private SKColorFilter _inverter;
    private bool          _rotate = true;
    private double        _rotation;
    private bool          _sign;

    /// <inheritdoc />
    protected override void OnLoad()
    {
        this._font.Hinting  = SKFontHinting.Full;
        this._font.Subpixel = true;

        var style = new Style
        {
            FontFamily = SKTypeface.Default.FamilyName,
            FontSize   = 16,
            TextColor  = SKColors.White,
        };

        this._textBlock.AddText( "Controls:\n",                           style );
        this._textBlock.AddText( "  - [1]: Toggle color inversion\n",     style );
        this._textBlock.AddText( "  - [2]: Reverse rotation direction\n", style );
        this._textBlock.AddText( "  - [3]: Pause\n",                      style );
        this._textBlock.AddText( "  - [Esc]: Exit",                       style );

        const string sksl =
            """

            // simple color inversion
            half4 main( in half4 color ) {
                return half4(1.0 - color.rgb, color.a);
            }
                    
            """;

        var effect = SKRuntimeEffect.CreateColorFilter( sksl, out var errors );
        if( errors is not null )
        {
            Console.Error.WriteLine( errors );
            Environment.Exit( 1 );
        }

        this._inverter = effect.ToColorFilter();

        base.OnLoad();
    }

    /// <inheritdoc />
    protected override void OnKeyUp( KeyboardKeyEventArgs e )
    {
        if( e is { Control: false, Command: false, Alt: false, Shift: false, Key: var k } )
        {
            switch( k )
            {
                case Keys.D1:
                    this._paint.ColorFilter =
                        this._paint.ColorFilter is null
                            ? this._inverter
                            : null;
                    break;

                case Keys.D2:
                    this._sign = !this._sign;
                    break;

                case Keys.D3:
                    this._rotate = !this._rotate;
                    break;

                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        base.OnKeyUp( e );
    }

    /// <inheritdoc />
    protected override void OnUpdateFrame( FrameEventArgs args )
    {
        this._index                = ( this._index + 1 ) % this._samples.Length;
        this._samples[this._index] = args.Time;

        if( this._rotate )
        {
            if( this._sign )
                this._rotation -= args.Time / 3;
            else
                this._rotation += args.Time / 3;
        }

        this._rotation %= 2 * Math.PI;

        base.OnUpdateFrame( args );
    }

    /// <inheritdoc />
    protected override void OnRenderFrame( FrameEventArgs args )
    {
        // rotate through the color spectrum
        var color = new SKColor(
            (byte)( 0xFF * Math.Abs( Math.Sin( this._rotation ) ) ),
            (byte)( 0xFF * Math.Abs( Math.Sin( this._rotation + 2 * Math.PI / 3 ) ) ),
            (byte)( 0xFF * Math.Abs( Math.Sin( this._rotation + 4 * Math.PI / 3 ) ) )
        );
        this.Canvas.Clear( color );

        this._textBlock.Layout();
        this._textBlock.Paint( this.Canvas, new( 10, 10 ), this._textPaint );

        var center = new Vector2( this.ClientSize.X / 2f, this.ClientSize.Y / 2f );

        var r = center.X * 0.66f;
        this._paint.Color = SKColors.Indigo;

        // square vertices
        var squareTopLeft     = new SKPoint( center.X - r, center.Y - r );
        var squareTopRight    = new SKPoint( center.X + r, center.Y - r );
        var squareBottomLeft  = new SKPoint( center.X - r, center.Y + r );
        var squareBottomRight = new SKPoint( center.X + r, center.Y + r );

        // rotate vertices around the center
        squareTopLeft     = RotatePoint( squareTopLeft,     center, this._rotation );
        squareTopRight    = RotatePoint( squareTopRight,    center, this._rotation );
        squareBottomLeft  = RotatePoint( squareBottomLeft,  center, this._rotation );
        squareBottomRight = RotatePoint( squareBottomRight, center, this._rotation );

        var path = new SKPath();
        path.MoveTo( squareTopLeft );
        path.LineTo( squareTopRight );
        path.LineTo( squareBottomRight );
        path.LineTo( squareBottomLeft );
        path.Close();

        this.Canvas.DrawPath( path, this._paint );

        // square vertices
        squareTopLeft     = new( center.X - r, center.Y - r );
        squareTopRight    = new( center.X + r, center.Y - r );
        squareBottomLeft  = new( center.X - r, center.Y + r );
        squareBottomRight = new( center.X + r, center.Y + r );

        // rotate vertices around the center in reverse
        squareTopLeft     = RotatePoint( squareTopLeft,     center, -this._rotation );
        squareTopRight    = RotatePoint( squareTopRight,    center, -this._rotation );
        squareBottomLeft  = RotatePoint( squareBottomLeft,  center, -this._rotation );
        squareBottomRight = RotatePoint( squareBottomRight, center, -this._rotation );

        path.Reset();
        path.MoveTo( squareTopLeft );
        path.LineTo( squareTopRight );
        path.LineTo( squareBottomRight );
        path.LineTo( squareBottomLeft );
        path.Close();

        this._paint.Color = SKColors.Goldenrod;
        this.Canvas.DrawPath( path, this._paint );

        var fps = 1f / this._samples.Where( x => x > Double.Epsilon ).Average();
        var str = $"Drawing at {Math.Ceiling( fps )} FPS!";
        this._font.MeasureText( str, out var bounds, this._paint );
        var pt = new SKPoint( center.X - bounds.Width / 2f, center.Y - bounds.Height / 2f );

        this._paint.Color = SKColors.White;
        this.Canvas.DrawText( str, pt, this._font, this._paint );

        base.OnRenderFrame( args );

        return;

        static SKPoint RotatePoint( SKPoint skPoint, Vector2 vector2, double rotation )
        {
            var cos = Math.Cos( rotation );
            var sin = Math.Sin( rotation );

            var x = (float)( vector2.X + ( skPoint.X - vector2.X ) * cos - ( skPoint.Y - vector2.Y ) * sin );
            var y = (float)( vector2.Y + ( skPoint.X - vector2.X ) * sin + ( skPoint.Y - vector2.Y ) * cos );

            return new( x, y );
        }
    }
}
