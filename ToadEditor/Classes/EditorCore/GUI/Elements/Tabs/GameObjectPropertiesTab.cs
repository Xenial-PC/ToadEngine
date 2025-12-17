using Prowl.Paper.Utilities;
using Prowl.Scribe;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Serializer;

namespace ToadEditor.Classes.EditorCore.GUI.Elements.Tabs;

public class GameObjectPropertiesTab(DockType dockType) : TabMenu(dockType, "Properties")
{
    private GameObject _lastSelected;
    private bool _isAlreadySelected;

    public void GetObjectInfo()
    {
        var gameObject = SceneHierarchyTab.SelectedGameObject;
        Serialize.DumpGameObjectSerializableTree(gameObject, $"{Directory.GetCurrentDirectory()}/test.txt");
        /*foreach (var component in gameObject.Components)
        {
            var value = SerializeBehavior.SerializeObject((component as Behavior)!);
            foreach (var field in value.Fields)
            {
                Console.WriteLine($"Name: {field.Key}, Value: {field.Value.Value}");
            }
        }*/
    }

    public override void TabBody(RectangleF containerSize)
    {
        /*UI.DrawRect(node.Rect, ColorUtil.FromArgb(255, 3, 3, 3));

        _isAlreadySelected = _lastSelected == SceneHierarchyTab.SelectedGameObject;
        if (!_isAlreadySelected)
        {
            GetObjectInfo();
            _lastSelected = SceneHierarchyTab.SelectedGameObject;
        }*/
    }
}
