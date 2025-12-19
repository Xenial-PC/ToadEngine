using Prowl.PaperUI;
using Prowl.Quill;
using Prowl.Scribe;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.UI;

public class Fonts
{
    public static FontFile Default = null!;
}

public class GUI
{
    public static CanvasRenderer CanvasRenderer;
    public static Paper UI;
   
    public static Action? GuiCallBack, StaticGuiCallBack;
    
    public static void Init(Window.Window window)
    {
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
        UI.BeginFrame(Time.DeltaTime);
        StaticGuiCallBack?.Invoke();
        GuiCallBack?.Invoke();
        UI.EndFrame();
    }

    public static void Dispose()
    {
        GuiCallBack = null!;
        CanvasRenderer.Dispose();
    }
}
