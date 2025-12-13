using Guinevere;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.GUI.Elements;

public class SceneHierarchyTab(DockSpaceManager.Docks dock) : TabMenu(dock)
{
    public static GameObject SelectedGameObject = null!;

    private Action? ChildrenCallback;

    public override void TabBody(Rect windowPos)
    {
        UI.DrawRect(windowPos, Color.FromArgb(255, 15, 15, 15));
        DrawGameObjectList(windowPos);
    }

    private void DrawGameObjectList(Rect windowPos)
    {
        float yGap = 10;
        foreach (var gameObject in GameObjects)
        {
            ChildrenCallback += () =>
            {
                windowPos.Y += yGap;
                CreateGameObjectView(gameObject.Value, windowPos);
                yGap += 10;
            };
        }

        using (UI.Node().Direction(Axis.Vertical).Gap(10).Enter())
        {
            ChildrenCallback?.Invoke();
        }

        ChildrenCallback = null;
    }

    private void CreateGameObjectView(GameObject go, Rect windowPos)
    {
        using (UI.Node())
        {
            var currentNode = UI.CurrentNode;
            currentNode.Rect.Width = windowPos.Width;
            currentNode.Rect.Height = 15;
            currentNode.Rect.X = windowPos.X;
            currentNode.Rect.Y = windowPos.Y;

            UI.DrawRect(currentNode.Rect, Color.FromArgb(255, 45, 45, 45));
            UI.DrawText($"{go.Name}", currentNode.Rect.X + 5f, currentNode.Center.Y + 2.5f, 12, SelectedGameObject == go ? Color.CornflowerBlue : Color.White);

            if (currentNode.GetInteractable().OnClick()) SelectedGameObject = go;
        }
    }

    public GameObject? FindGameObject(string name) => Service.Scene.ObjectManager.FindGameObject(name);
    public Dictionary<string, GameObject> GameObjects => Service.Scene.ObjectManager.GameObjects;
}
