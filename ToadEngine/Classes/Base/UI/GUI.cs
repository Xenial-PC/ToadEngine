using Prowl.PaperUI;
using Prowl.Quill;
using Prowl.Scribe;
using ToadEngine.Classes.Base.Scripting.Base;
using ImGuiController = ToadEngine.Classes.DearImGui.OpenTK.ImGuiController;

namespace ToadEngine.Classes.Base.UI;

public class GUI
{
    public static ImGuiController Controller = null!;
    public static ImPlotContext ImPlotContext = null!;

    public static CanvasRenderer CanvasRenderer;
    public static Paper UI;
   
    public static Action? GuiCallBack = null!;
   
    public class Fonts
    {
        public static FontFile Default = null!;
    }

    public static void Init(Window.Window window)
    {
        Controller = new ImGuiController(window, $"Roboto-Regular.ttf", 20, 20.0f);
        ImPlotContext = ImPlot.CreateContext();
        ImPlot.SetCurrentContext(ImPlotContext);
        ImPlot.SetImGuiContext(Controller.Context);

        CanvasRenderer = new CanvasRenderer();
        CanvasRenderer.Initialize(window.Width, window.Height);

        UI = new Paper(CanvasRenderer, window.Width, window.Height, new FontAtlasSettings());

        var fontStream = RReader.ReadBytes($"font.ttf");
        if (fontStream == null) return;

        Fonts.Default = new FontFile(fontStream);
        UI.Canvas.AddFallbackFont(Fonts.Default);
        UI.AddFallbackFont(Fonts.Default);
    }

    public static void Render()
    {
        Controller.Update(Time.DeltaTime);
        UI.BeginFrame(Time.DeltaTime);
        GuiCallBack?.Invoke();
        UI.EndFrame();
        Controller.Render();
    }

    public static void Dispose()
    {
        GuiCallBack = null!;
    }
}
