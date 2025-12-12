using Guinevere;
using ToadEngine.Classes.Base.Scripting.Base;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

namespace ToadEditor.Classes.EditorCore.GUI;

public class GUIHelpers
{
    private static bool _isResizing;
    private static bool _resizeStarted;
    private static Vector2 _lastMouse;
    
    private static bool _isMoving;
    private static Vector2 _moveOffset;

    private const int ResizeMargin = 8;

    public static Rect ResizeElement(Rect rect, bool isHeld)
    {
        if (_isMoving) return rect;
        var mouse = Input.GetMousePos();
        
        if (!isHeld)
        {
            _isResizing = false;
            _resizeStarted = false;
            return rect;
        }

        if (!_isResizing)
        {
            _isResizing = true;
            _resizeStarted = false;
            _lastMouse = mouse;
            return rect;
        }

        var delta = mouse - _lastMouse;

        if (!_resizeStarted)
        {
            if (Math.Abs(delta.X) < 1f && Math.Abs(delta.Y) < 1f)
                return rect;

            _resizeStarted = true;
        }

        rect.Width += delta.X;
        rect.Height += delta.Y;

        _lastMouse = mouse;

        rect.Width = Math.Max(50, rect.Width);
        rect.Height = Math.Max(50, rect.Height);

        return rect;
    }

    public static Rect MoveElement(Rect rect, bool isHeld)
    {
        if (_isResizing) return rect;
        var mouse = Input.GetMousePos();

        if (!isHeld)
        {
            _isMoving = false;
            return rect;
        }

        if (!_isMoving)
        {
            _isMoving = true;
            _moveOffset = mouse - new Vector2(rect.X, rect.Y);
            return rect;
        }

        rect.X = mouse.X - _moveOffset.X;
        rect.Y = mouse.Y - _moveOffset.Y;
        return rect;
    }
}
