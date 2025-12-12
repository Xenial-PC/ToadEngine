using Guinevere;
using ToadEditor.Classes.EditorCore.Renderer;
using ToadEngine.Classes.Base.Scripting.Base;
using MouseButton = Guinevere.MouseButton;
using Vector2 = System.Numerics.Vector2;
using Window = ToadEngine.Classes.Window.Window;

namespace ToadEditor.Classes.EditorCore.GUI;

public class SceneViewWindow(EditorRenderTarget target, Window window)
{
    public nint TextureId;
    public int Width = 200, Height = 200;
    public bool IsSceneOpen = true;

    private Gui Paint => ToadEngine.Classes.Base.UI.GUI.Paint;

    private Rect _renderSceneWindow = null!;

    public void Setup()
    {
        var sceneWindowWidth = 400;
        var sceneWindowHeight = 400;
        _renderSceneWindow = new Rect(X: window.Width / 2f - sceneWindowWidth / 2f, Y: 20, W: sceneWindowWidth,
            H: sceneWindowHeight);
    }

    public void DrawSceneViewWindow()
    {
        var currentNode = Paint.Node(100, 100);
        var windowPos = _renderSceneWindow;
        currentNode.Rect = windowPos;

        using (currentNode.Enter())
        {
            using (Paint.Node(windowPos.Width, 25).Enter())
            {
                var headerNode = Paint.CurrentNode;
                headerNode.Rect.X = windowPos.X;
                headerNode.Rect.Y = windowPos.Y - headerNode.Rect.Height + 5f;

                var isHeld = headerNode.GetInteractable().OnHold(MouseButton.Left);
                _renderSceneWindow = GUIHelpers.MoveElement(windowPos, isHeld);

                Paint.DrawRect(headerNode.Rect, Color.FromArgb(255, 5, 5, 5), 3);

                using (Paint.Node(headerNode.Rect.W, headerNode.Rect.H).Enter())
                {
                    var headerTextNode = Paint.CurrentNode;
                    headerTextNode.Rect = headerNode.Rect;

                    var xPos = headerTextNode.Rect.X + 35f;
                    var yPos = headerTextNode.Center.Y + 5f;
                    Paint.DrawText("Scene View", xPos, yPos, 15, Color.White);
                }
            }

            using (Paint.Node(20, 20).Enter())
            {
                var windowStateNode = Paint.CurrentNode;
                windowStateNode.Rect.X = windowPos.X;
                windowStateNode.Rect.Y = windowPos.Y - 10f;

                var isClicked = windowStateNode.GetInteractable().OnClick();
                if (isClicked) IsSceneOpen = !IsSceneOpen;

                Paint.DrawText(IsSceneOpen ? "-" : "+", windowStateNode.Center.X, windowStateNode.Center.Y, 20,
                    Color.White);
            }

            if (!IsSceneOpen) return;
            using (Paint.Node(10, 10).Enter())
            {
                var resizeNode = Paint.CurrentNode;
                resizeNode.Rect.X = (windowPos.X + windowPos.Width) - resizeNode.Rect.Width;
                resizeNode.Rect.Y = (windowPos.Y + windowPos.Height) - resizeNode.Rect.Height;

                Paint.DrawRect(resizeNode.Rect, Color.CornflowerBlue, 2);

                var isHeld = resizeNode.GetInteractable().OnHold(MouseButton.Left);
                _renderSceneWindow = GUIHelpers.ResizeElement(windowPos, isHeld);
            }

            Paint.DrawRect(windowPos, Color.FromArgb(255, 10, 10, 10), 3);
            RenderSceneViewWindow(windowPos);
        }
    }

    private void RenderSceneViewWindow(Rect sceneViewRect)
    {
        if (Paint.Pass != Pass.Pass2Render) return;
        ImGui.SetNextWindowPos(new Vector2(sceneViewRect.X, sceneViewRect.Y));
        ImGui.SetNextWindowSize(new Vector2(sceneViewRect.Width, sceneViewRect.Height));

        ImGui.Begin("Scene View", ref IsSceneOpen,
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

            Service.MainCamera.AspectRatio = (Width / (float)Height);
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
