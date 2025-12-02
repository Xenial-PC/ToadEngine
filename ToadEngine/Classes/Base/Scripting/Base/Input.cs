namespace ToadEngine.Classes.Base.Scripting.Base;

public static class Input
{
    private static NativeWindow WHandler => Service.NativeWindow;

    public static bool IsAnyKeyDown => WHandler.IsAnyKeyDown;
    public static bool IsAnyMouseButtonDown => WHandler.IsAnyMouseButtonDown;

    public static CursorState CursorState { get => WHandler.CursorState; set => WHandler.CursorState = value; }

    public static bool IsKeyDown(Keys key) => WHandler.KeyboardState.IsKeyDown(key);
    public static bool IsKeyReleased(Keys key) => WHandler.KeyboardState.IsKeyReleased(key);
    public static bool IsKeyPressed(Keys key) => WHandler.KeyboardState.IsKeyPressed(key);
   
    public static bool IsMouseDown(MouseButton button) => WHandler.MouseState.IsButtonDown(button);
    public static bool IsMousePressed(MouseButton button) => WHandler.MouseState.IsButtonPressed(button);
    public static bool IsMouseReleased(MouseButton button) => WHandler.MouseState.IsButtonReleased(button);
    
    public static Vector2 GetMousePos() => WHandler.MouseState.Position;
    public static void SetMousePos(Vector2 pos) => WHandler.MousePosition = pos;
}
