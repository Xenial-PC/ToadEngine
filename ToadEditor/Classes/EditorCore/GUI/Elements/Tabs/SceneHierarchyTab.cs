using Prowl.PaperUI;
using Prowl.Scribe;
using Prowl.Vector;
using ToadEditor.Classes.EditorCore.GUI.Base;
using ToadEditor.Classes.EditorCore.GUI.Components;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using TextAlignment = Prowl.PaperUI.TextAlignment;

namespace ToadEditor.Classes.EditorCore.GUI.Elements.Tabs;

public class SceneHierarchyTab(DockType dockType) : TabMenu(dockType, "Hierarchy")
{
    public static GameObject SelectedGameObject = null!;
    private string _filter = string.Empty;

    public override void TabBody(RectangleF containerSize)
    {
        using (UI.Column("BodyContainer").Height(UI.Stretch()).Enter())
        {
            DrawSearchbar();
            DrawGameObjectList();
        }
    }

    private void DrawSearchbar()
    {
        UI.Box("Searchbar").Height(20).Width(UI.Stretch(1f))
            .Rounded(5f)
            .TextField("", Fonts.Default, inputString =>
            {
                _filter = inputString;
            }, Color.Black, "Search...")
            .TextColor(Color.Black)
            .BackgroundColor(Color.White).Margin(UI.Stretch(0.15f), UI.Stretch(0.15f), 15f, 5f)
            .Alignment(TextAlignment.MiddleCenter)
            .FontSize(15f);
    }

    private void DrawGameObjectList()
    {
        using (UI.Column("GameObjects").SetScroll(Scroll.ScrollXY).Height(UI.Stretch()).Clip()
                   .Enter())
        {
            foreach (var gameObject in GameObjects.Where(goName => string.IsNullOrEmpty(_filter) || goName.Value.Name!.Contains(_filter)))
            {
                var go = gameObject.Value;
                if (go.IsChild) continue;

                var childIndent = 0;
                DrawGameObject(go, ref childIndent);
            }

            UI.Box("BottomPadding").Height(40);
        }
    }

    private void DrawGameObject(GameObject go, ref int childIndent)
    {
        using (UI.Box(go.Name!)
                   .BackgroundColor(Color32.FromArgb(5, 25, 25, 45))
                   .Height(15)
                   .Rounded(3f)
                   .Top(5f)
                   .Left(5f)
                   .If(go.IsChild)
                   .Width(UI.Percent(100f))
                   .Left(childIndent)
                   .End()
                   .OnClick((e) =>
                   {
                       if (e.Button != PaperMouseBtn.Left) return;
                       SelectedGameObject = go;
                   })
                   .If(go == SelectedGameObject)
                   .BackgroundColor(Color32.FromArgb(100, Color.Aqua))
                   .End()
                   .Enter())
        {
            using (UI.Row("GOElements").Enter())
            {
                UI.Box("DropDownText").Text(go.HasChildren ? ">" : "", Fonts.Default).TextColor(Color.White)
                    .FontSize(15f).Width(5f).Left(5f).OnClick((e) =>
                    {
                        if (e.Button != PaperMouseBtn.Left) return;
                                
                    });

                UI.Box("NameText").Text(go.Name!, Fonts.Default)
                    .TextColor(Color.White).Left(5f).Alignment(TextAlignment.MiddleLeft);
            }
        }

        if (!go.HasChildren)
        {
            childIndent = 10;
            return;
        }

        childIndent += 10;
        foreach (var child in go.Children)
            DrawGameObject(child, ref childIndent);
    }

    public GameObject? FindGameObject(string name) => Service.Scene.ObjectManager.FindGameObject(name);
    public Dictionary<string, GameObject>? GameObjects => Service.Scene == null ? new Dictionary<string, GameObject>() : Service.Scene.ObjectManager.GameObjects;
}
