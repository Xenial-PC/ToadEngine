using Guinevere;
using SkiaSharp;
using ImGuiController = ToadEngine.Classes.DearImGui.OpenTK.ImGuiController;

namespace ToadEngine.Classes.Base.UI;

public class GUI
{
    public static ImGuiController Controller;
    public static ImPlotContext ImPlotContext;

    public static Gui Paint;
    public static ICanvasRenderer Canvas;
    public static Action GuiCallBack;

    public static Font FontText, FontIcon;
    public static StringBuilder TypedCharacters = new();

    public static void Init(Window.Window window)
    {
        Controller = new ImGuiController(window, $"Roboto-Regular.ttf", 20, 20.0f);
        ImPlotContext = ImPlot.CreateContext();
        ImPlot.SetCurrentContext(ImPlotContext);
        ImPlot.SetImGuiContext(Controller.Context);

        Paint = new Gui
        {
            Input = window,
            WindowHandler = window
        };

        var fontStream = RReader.GetStream($"font.ttf");
        FontText = Font.FromStream(fontStream);

        fontStream = RReader.GetStream($"icons.ttf");
        FontIcon = Font.FromStream(fontStream);

        Canvas = new CanvasRenderer();
        Canvas.Initialize(window.Width, window.Height);

        window.TextInput += WindowOnTextInput;
    }

    private static void WindowOnTextInput(TextInputEventArgs obj)
    {
        
    }

    public static void Render()
    {
        Canvas.Render(canvas =>
        {
            Paint.SetStage(Pass.Pass1Build);
            Paint.BeginFrame(canvas, FontText, FontIcon);
            GuiCallBack();

            Paint.CalculateLayout();

            Paint.SetStage(Pass.Pass2Render);
            GuiCallBack();
            Paint.Render();

            Paint.EndFrame();
        });
    }

    public static void Dispose()
    {
        GuiCallBack = null!;
        Canvas.Dispose();
        FontText.Dispose();
        FontIcon.Dispose();
    }
}
