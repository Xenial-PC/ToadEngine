using Guinevere;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEngine.Classes.Base.Scripting.Base;
using Vector2 = System.Numerics.Vector2;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class SceneViewTab(EditorRenderTarget target, DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public nint TextureId;
    public int Width = 200, Height = 200;

    public override void TabBody(Rect windowPos)
    {
        UI.DrawRect(windowPos, Color.Black, 3);
        RenderSceneViewWindow(windowPos);
    }

    private void RenderSceneViewWindow(Rect sceneViewRect)
    {
        if (UI.Pass != Pass.Pass2Render) return;
        ImGui.SetNextWindowPos(new Vector2(sceneViewRect.X, sceneViewRect.Y));
        ImGui.SetNextWindowSize(new Vector2(sceneViewRect.Width, sceneViewRect.Height));

        ImGui.Begin("Scene View", ref IsSelected,
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoBackground);

        var size = ImGui.GetContentRegionAvail();
        var w = (int)size.X;
        var h = (int)size.Y;

        if (w > 0 && h > 0 && (w != Width || h != Height))
        {
            Width = w;
            Height = h;

            target.Resize(w, h);

            Service.MainCamera.AspectRatio = Width / (float)Height;
            TextureId = target.Texture;
        }

        ImGui.Image(
            TextureId,
            new Vector2(Width, Height),
            new Vector2(0, 1),
            new Vector2(1, 0)
        );

        ImGui.End();
    }
}
