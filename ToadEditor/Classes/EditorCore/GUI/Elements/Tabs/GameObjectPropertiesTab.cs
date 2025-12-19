using Prowl.Echo;
using Prowl.Paper.Utilities;
using Prowl.Scribe;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.GUI.Elements.Tabs;

public class GameObjectPropertiesTab(DockType dockType) : TabMenu(dockType, "Properties")
{
    private GameObject _lastSelected;
    private bool _isAlreadySelected;

    public void GetObjectInfo()
    {
        var gameObject = SceneHierarchyTab.SelectedGameObject;
        var serialized = gameObject.Serialized;

        /*var obj = Serializer.Deserialize<GameObject>(serialized);
        Service.Scene.Instantiate(obj);*/
    }

    public override void TabBody(RectangleF containerSize)
    {
        _isAlreadySelected = _lastSelected == SceneHierarchyTab.SelectedGameObject;
        if (!_isAlreadySelected)
        {
            GetObjectInfo();
            _lastSelected = SceneHierarchyTab.SelectedGameObject;
        }
    }
}
