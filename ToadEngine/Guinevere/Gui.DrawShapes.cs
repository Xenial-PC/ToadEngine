using System.Drawing;
using System.Runtime.CompilerServices;

namespace Guinevere;

/// <summary>
/// Represents the main GUI class for drawing shapes and managing graphical objects.
/// </summary>
public partial class Gui
{
    private readonly Dictionary<string ,SKImage> _imageList = new();
    private readonly List<SKImage> _imageCache = new();

    /// <summary>
    /// Draws a rectangle border
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape DrawRectBorder(Rect screenRect, Color color, float thickness = 1f,
        float radius = 0.0f,
        Corner corners = Corner.All)
    {
        return DrawRectBorder(
            new Vector2(screenRect.X, screenRect.Y),
            new Vector2(screenRect.W, screenRect.H),
            color, thickness, radius, corners);
    }

    /// <summary>
    /// Draws a rectangle border
    /// </summary>
    [PublicAPI]
    public Shape DrawRectBorder(Vector2 topLeft, Vector2 size, Color color, float thickness = 1f,
        float radius = 0.0f,
        Corner corners = Corner.All)
    {
        var bottomRight = new Vector2(topLeft.X + size.X, topLeft.Y + size.Y);
        var shape = DrawRect(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, radius, corners);
        shape.Stroke(color, thickness);
        AddDraw(shape);
        return shape;
    }

    private Shape DrawRect(float x, float y, float w, float h, float radius = 0.0f, Corner corners = Corner.All)
    {
        return ImMath.ApproximatelyEquals(radius, 0.0f)
            ? Shape.Rect(x, y, w, h)
            : Shape.RoundRect(x, y, w, h, radius, corners);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    [PublicAPI]
    public Shape DrawRectFilled(
        Rect screenRect,
        Color? color,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        return DrawRectFilled(
            screenRect.X, screenRect.Y,
            screenRect.BottomRight.X, screenRect.BottomRight.Y,
            color, radius, corners);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    [PublicAPI]
    public Shape DrawRectFilled(
        float x, float y, float w, float h,
        Color? color,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        var shape = DrawRect(x, y, w, h, radius, corners);
        if (color != null)
            shape.SolidColor(color.Value);
        AddDraw(shape);
        return shape;
    }

    /// <summary>
    /// Draws a filled triangle
    /// </summary>
    [PublicAPI]
    public Shape DrawTriangleFilled(
        Vector2 a, Vector2 b, Vector2 c,
        Color colorA, Color? colorB = null, Color? colorC = null)
    {
        var shape = Shape.Triangle(a, b, c);

        // Handle gradient coloring if multiple colors provided
        if (colorB.HasValue || colorC.HasValue)
        {
            var color2 = colorB ?? colorA;
            // var color3 = colorC ?? colorA;

            // Use linear gradient as an approximation for multicolor triangle
            shape.LinearGradientColor(colorA, color2);
        }
        else
        {
            shape.SolidColor(colorA);
        }

        AddDraw(shape);
        return shape;
    }

    /// <summary>
    /// Draws a filled rectangle (alias for DrawRectFilled)
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape DrawRect(
        Rect rect,
        Color color,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        return DrawRectFilled(rect.X, rect.Y, rect.BottomRight.X, rect.BottomRight.Y, color, radius, corners);
    }

    /// <summary>
    /// Draws a filled rectangle (alias for DrawRectFilled)
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape DrawRect(
        Vector2 position, Vector2 size,
        Color color,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        return DrawRectFilled(position.X, position.Y, size.X, size.Y, color, radius, corners);
    }

    /// <summary>
    /// Draws a filled rectangle (alias for DrawRectFilled)
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape DrawRect(Rect rect,
        Color? color = null,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        return DrawRectFilled(rect.Position.X, rect.Size.Y, rect.BottomRight.X, rect.BottomRight.Y, color, radius,
            corners);
    }


    /// <summary>
    /// Draws a circle border
    /// </summary>
    [PublicAPI]
    public Shape DrawCircleBorder(Vector2 center, float radius, Color color, float thickness = 1f)
    {
        var shape = Shape.Circle(radius, center);
        shape.Stroke(color, thickness);
        AddDraw(shape);
        return shape;
    }

    /// <summary>
    /// Draws a filled circle
    /// </summary>
    [PublicAPI]
    public Shape DrawCircleFilled(Vector2 center, float radius,
        Color color)
    {
        var shape = Shape.Circle(radius, center);
        shape.SolidColor(color);
        AddDraw(shape);
        return shape;
    }

    /// <summary>
    /// Draws a filled circle (alias for DrawCircleFilled)
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape DrawCircle(Vector2 center, float radius,
        Color color)
    {
        return DrawCircleFilled(center, radius, color);
    }

    /// <summary>
    /// Draws a filled triangle (alias for DrawTriangleFilled)
    /// </summary>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c,
        Color colorA, Color? colorB = null, Color? colorC = null)
    {
        DrawTriangleFilled(a, b, c, colorA, colorB, colorC);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    [PublicAPI]
    public Shape DrawBackgroundRect(
        Color? color = null,
        float radius = 0.0f, Corner corners = Corner.All)
    {
        var shape = DrawRectFilled(CurrentNode.Rect, color, radius, corners);
        shape.SolidColor(color);
        AddDraw(shape, true);
        return shape;
    }

    // /// <summary>
    // /// Draws a filled rectangle
    // /// </summary>
    // [PublicAPI]
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public Shape DrawBackgroundRect(
    //     float radius = 0.0f, Corner corners = Corner.All) =>
    //     DrawBackgroundRect(Color.White, radius, corners);

    /// <summary>
    /// Draws a line between two points
    /// </summary>
    [PublicAPI]
    public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1f)
    {
        var paint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = thickness,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };

        if (Pass == Pass.Pass2Render) Canvas!.DrawLine(start.X, start.Y, end.X, end.Y, paint);
    }

    /// <summary>
    /// Draws text at a certain Position
    /// </summary>
    /// <param name="text"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="font"></param>
    /// <param name="clip"></param>
    public void DrawText(
        string text,
        float x,
        float y,
        float size = 0,
        Color? color = null,
        Font? font = null,
        bool clip = false)
    {
        var resolvedFont = font ?? CurrentNodeScope.Get<LayoutNodeScopeTextFont>().Value;
        var resolvedSize = size > 0 ? size : CurrentNodeScope.Get<LayoutNodeScopeTextSize>().Value;
        var f = new SKFont(resolvedFont.SkFont.Typeface, resolvedSize);

        var paint = new SKPaint { IsAntialias = true, Color = color ?? CurrentNodeScope.Get<LayoutNodeScopeTextColor>().Value };

        AddDraw(new Text(text, new Vector2(x, y), f, paint), clip, CurrentNode);
    }

    /// <summary>
    /// Draws an image on the screen at an absolute position
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="x">X position absolute</param>
    /// <param name="y">Y position absolute</param>
    /// <param name="width">Image Width</param>
    /// <param name="height">Image Height</param>
    /// <param name="color">Color</param>
    /// <param name="clip">Prepend the image</param>
    public void DrawImage(
        string image,
        float x,
        float y,
        float width,
        float height,
        Color? color = null,
        bool clip = false)
    {
        if (!_imageList.TryGetValue(image, out var eImage))
        {
            eImage = SKImage.FromEncodedData(File.ReadAllBytes(image));
            _imageList.Add(image, ImageDrawable.ConvertToBgra(eImage));
        }

        var paint = new SKPaint { IsAntialias = true, Color = color ?? Color.White };
        var rect = new Rect(x, y, width, height);
        var drawable = new ImageDrawable(eImage, rect, paint);

        AddDraw(drawable, clip, CurrentNode);
    }

    /// <summary>
    /// Draws an image on the screen at an absolute position
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="x">X position absolute</param>
    /// <param name="y">Y position absolute</param>
    /// <param name="width">Image Width</param>
    /// <param name="height">Image Height</param>
    /// <param name="color">Color</param>
    /// <param name="clip">Prepend the image</param>
    public void DrawImage(
        ref SKImage image,
        float x,
        float y,
        float width,
        float height,
        Color? color = null,
        bool clip = false)
    {
        if (!_imageCache.Contains(image))
        {
            image = ImageDrawable.ConvertToBgra(image);
            _imageCache.Add(image);
        }

        var imgRef = image;
        image = _imageCache.First(img => img == imgRef);

        var paint = new SKPaint { IsAntialias = true, Color = color ?? Color.White };
        var rect = new Rect(x, y, width, height);
        var drawable = new ImageDrawable(image, rect, paint);

        AddDraw(drawable, clip, CurrentNode);
    }

    /// <summary>
    /// Draws a shape at the specified position and applies transformations.
    /// </summary>
    /// <param name="position">The position where the shape should be drawn.</param>
    /// <param name="shape">The shape to be drawn, which will be transformed and rendered.</param>
    /// <returns>The newly created and rendered shape.</returns>
    public Shape DrawShape(Vector2 position, Shape shape)
    {
        var newShape = shape.Copy();
        newShape.Node = CurrentNode;
        foreach (var (_, layerList) in newShape.Layers)
        foreach (var (layerPath, _) in layerList)
            layerPath.Transform(SKMatrix.CreateTranslation(
                position.X,
                position.Y));

        AddDraw(newShape);
        return newShape;
    }

    /// <summary>
    /// Sets a clipping area for rendering content inside a specific layout node.
    /// </summary>
    /// <param name="node">The layout node to which the clipping area is applied.</param>
    /// <param name="clipShape">The shape defining the clipping area.</param>
    public void SetClipArea(LayoutNode node, Shape clipShape)
    {
        if (Pass != Pass.Pass2Render) return;

        // Queue the clip operation in the node's draw list
        node.DrawList.AddClip(clipShape, node.Rect.Center);
    }

    private void AddDraw(IDrawable shape, bool prepend = false, LayoutNode? node = null)
    {
        if (Pass != Pass.Pass2Render) return;
        node ??= CurrentNode;
        if (prepend)
            node.DrawList.Prepend(shape);
        else
            node.DrawList.Add(shape);
    }

    private void AddDraw(IDrawListEntry entry, LayoutNode? node = null, bool prepend = false)
    {
        if (Pass != Pass.Pass2Render) return;
        node ??= CurrentNode;
        if (prepend)
            node.DrawList.Prepend(entry);
        else
            node.DrawList.Add(entry);
    }
}
