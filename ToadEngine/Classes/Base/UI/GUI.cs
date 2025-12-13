using Guinevere;
using ImGuiController = ToadEngine.Classes.DearImGui.OpenTK.ImGuiController;

namespace ToadEngine.Classes.Base.UI;

public class GUI
{
    public static ImGuiController Controller = null!;
    public static ImPlotContext ImPlotContext = null!;

    public static Gui UI = null!;
    public static ICanvasRenderer Canvas = null!;
    public static Action? GuiCallBack = null!;

    public static Font FontText = null!, FontIcon = null!;
    public static StringBuilder TypedCharacters = new();

    public static void Init(Window.Window window)
    {
        Controller = new ImGuiController(window, $"Roboto-Regular.ttf", 20, 20.0f);
        ImPlotContext = ImPlot.CreateContext();
        ImPlot.SetCurrentContext(ImPlotContext);
        ImPlot.SetImGuiContext(Controller.Context);

        UI = new Gui
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
            Controller.Update(UI.Time.DeltaTime);

            UI.SetStage(Pass.Pass1Build);
            UI.BeginFrame(canvas, FontText, FontIcon);
            GuiCallBack?.Invoke();

            UI.CalculateLayout();
            UI.SetStage(Pass.Pass2Render);
            GuiCallBack?.Invoke();

            UI.Render();
            UI.EndFrame();
        });

        Controller.Render();
    }

    public static void Dispose()
    {
        GuiCallBack = null!;
        Canvas.Dispose();
        FontText.Dispose();
        FontIcon.Dispose();
    }
}
