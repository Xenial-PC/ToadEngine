namespace Guinevere;

public class ImageDrawable : IDrawable
{
    public SKImage Image { get; }
    public Rect Destination { get; }

    public SKPaint? Paint { get; }

    public ImageDrawable(SKImage image, Rect dest, SKPaint paint)
    {
        Image = image;
        Destination = dest;
        Paint = paint;
    }

    public void Render(Gui gui, LayoutNode node, SKCanvas canvas)
    {
        var r = Destination;
        var skRect = new SKRect(r.X, r.Y, r.X + r.W, r.Y + r.H);
        canvas.DrawImage(Image, skRect, Paint);
    }

    public static SKImage ConvertToBgra(SKImage src)
    {
        var info = new SKImageInfo(src.Width, src.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);

        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.DrawImage(src, 0, 0);

        return surface.Snapshot();
    }
}