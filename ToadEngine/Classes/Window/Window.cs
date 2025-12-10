using Guinevere;
using OpenTK.Platform.Windows;
using SkiaSharp;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using MouseButton = Guinevere.MouseButton;

namespace ToadEngine.Classes.Window;

public class Window : GameWindow, IInputHandler, IWindowHandler
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Shader CoreShader = null!;

    public Scene CurrentScene { get; set; } = new();

    public Window(int width, int height, string title) :
        base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title, NumberOfSamples = 4, Vsync = VSyncMode.Off })
    {
        Width = width;
        Height = height;

        GUI.Init(this);
        Run();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        Setup();
        OnInit();
    }

    public virtual void Setup()
    {
        UpdateFrequency = 120;

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.FramebufferSrgb);
    }

    public virtual void OnInit()
    {
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        OnDraw(e);

        GUI.Render();
        SwapBuffers();
    }

    public virtual void OnDraw(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        CoreShader.Use();

        CurrentScene.Draw((float)e.Time);

        GUI.Controller.Render();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        GUI.Controller!.Update((float)e.Time);
        GUI.Paint.Time.Update(e.Time);
        OnUpdate(e);
    }

    public virtual void OnUpdate(FrameEventArgs e)
    {
        CurrentScene.Update(e);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        OnDispose();
    }

    public virtual void OnDispose()
    {
        CoreShader.Dispose();
        ImPlot.DestroyContext(GUI.ImPlotContext);
        GUI.Controller.Dispose();
        GUI.Dispose();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        GUI.Canvas.Resize(e.Width, e.Height);
        OnResize(e);
    }

    public virtual void OnResize(FramebufferResizeEventArgs e)
    {
        Width = e.Width;
        Height = e.Height;

        CurrentScene.OnResize(e);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(e.AsString))
            {
                GUI.TypedCharacters.Append(e.AsString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Text input error: {ex.Message}");
        }
    }

    public void LoadScene(string name)
    {
        CurrentScene.Destroy();
        CoreShader = new Shader($"core.vert", $"lighting.frag");
        CoreShader.Use();

        Service.Add(CoreShader);

        CurrentScene = SceneManager.Create(name);
        Service.Add(CurrentScene);

        CurrentScene.Load(this, this);
    }

    #region IInputHandler

    public Vector2 MouseDelta => new(MouseState.Delta.X, MouseState.Delta.Y);

    public new System.Numerics.Vector2 MousePosition => new(MouseState.Position.X, MouseState.Position.Y);

    public float MouseWheelDelta => MouseState.ScrollDelta.Y;

    public Vector2 PrevMousePosition => new(MouseState.PreviousPosition.X, MouseState.PreviousPosition.Y);

    public new bool IsAnyKeyDown => KeyboardState.IsAnyKeyDown;

    public bool IsKeyPressed(KeyboardKey keyboardKey) =>
        IsKeyPressed((Keys)(int)keyboardKey);

    public bool IsKeyDown(KeyboardKey keyboardKey) =>
        IsKeyDown((Keys)(int)keyboardKey);

    public bool IsKeyUp(KeyboardKey keyboardKey) =>
        IsKeyReleased((Keys)(int)keyboardKey);

    public bool IsMouseButtonPressed(MouseButton button) =>
        IsMouseButtonPressed((global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)(int)button);

    public bool IsMouseButtonDown(MouseButton button) =>
        IsMouseButtonDown((global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)(int)button);

    public bool IsMouseButtonUp(MouseButton button) =>
        IsMouseButtonReleased((global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)(int)button);

    public string GetTypedCharacters()
    {
        try
        {
            var result = GUI.TypedCharacters.ToString();
            GUI.TypedCharacters.Clear();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetTypedCharacters error: {ex.Message}");
            GUI.TypedCharacters.Clear();
            return "";
        }
    }

    public unsafe string GetClipboardText()
    {
        try
        {
            var result = GLFW.GetClipboardString(WindowPtr);
            return result ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Clipboard get error: {ex.Message}");
            return "";
        }
    }

    public unsafe void SetClipboardText(string text)
    {
        try
        {
            if (!string.IsNullOrEmpty(text))
            {
                GLFW.SetClipboardString(WindowPtr, text);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Clipboard set error: {ex.Message}");
        }
    }

    #endregion IInputHandler

    public void DrawWindowTitlebar(bool show) =>
        WindowBorder = show ? WindowBorder.Resizable : WindowBorder.Hidden;
}
