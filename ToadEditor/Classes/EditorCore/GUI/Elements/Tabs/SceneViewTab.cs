using Prowl.Scribe;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Textures;

namespace ToadEditor.Classes.EditorCore.GUI.Elements.Tabs;

public class SceneViewTab(EditorRenderTarget target, DockType dockType) : TabMenu(dockType, "Scene View")
{
    public Texture Texture = null!;
    public int Width, Height;

    public Action? SceneViewOverlay;

    public override void TabBody(RectangleF containerSize)
    {
        RenderSceneViewWindow(containerSize);
    }

    private void RenderSceneViewWindow(RectangleF containerSize)
    {
        if (Width != (int)containerSize.Width || Height != (int)containerSize.Height)
        {
            Texture = target.Resize((int)containerSize.Width, (int)containerSize.Height);

            if (Camera.MainCamera != null!)
                Camera.MainCamera.AspectRatio = target.Width / (float)target.Height;

            Width = (int)containerSize.Width;
            Height = (int)containerSize.Height;
        }

        UI.Box("SceneRenderer").Image(Texture);
        SceneViewOverlay?.Invoke();
    }
}
