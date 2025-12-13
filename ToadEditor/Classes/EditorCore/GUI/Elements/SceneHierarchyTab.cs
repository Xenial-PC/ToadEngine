using Guinevere;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class SceneHierarchyTab(DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public static GameObject SelectedGameObject = null!;

    public override void TabBody(LayoutNode node)
    {
        UI.DrawRect(node.Rect, Color.FromArgb(255, 3, 3, 3));
        DrawGameObjectList(node);
    }

    private void DrawGameObjectList(LayoutNode node)
    {
        using (UI.Node(node.Rect.Width, node.Rect.Height).Top(node.Rect.Y).Left(node.Rect.X).Direction(Axis.Vertical).Gap(10).Enter())
        {
            var parentNode = UI.CurrentNode;
            using (UI.Node(node.Rect.Width, node.Rect.Height).Top(parentNode.Rect.Y).Left(parentNode.Rect.X).Enter())
            {
                float gap = 10;
                foreach (var gameObject in GameObjects)
                {
                    using (UI.Node().Height(20).Top(parentNode.Rect.Y + gap).Enter())
                    {
                        var go = gameObject.Value;
                        var currentNode = UI.CurrentNode;

                        UI.DrawRect(currentNode.Rect, Color.FromArgb(255, 45, 45, 45), 12);
                        UI.DrawText($"{go.Name}", currentNode.Rect.X + 30f, currentNode.Center.Y + 2.5f, 12, SelectedGameObject == go ? Color.CornflowerBlue : Color.White);

                        if (currentNode.GetInteractable().OnClick()) SelectedGameObject = go;
                    }
                    gap += 30;
                }
            }
        }
    }

    public GameObject? FindGameObject(string name) => Service.Scene.ObjectManager.FindGameObject(name);
    public Dictionary<string, GameObject> GameObjects => Service.Scene.ObjectManager.GameObjects;
}
