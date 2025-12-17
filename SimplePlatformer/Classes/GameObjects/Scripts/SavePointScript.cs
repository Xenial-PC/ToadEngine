using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Event;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class SavePointScript : MonoBehavior
{
    public Trigger Trigger = null!;
    public TriggerScript TriggerScript = null!;
    public BoxCollider Collider = null!;

    public static Vector3 SavePoint;
    public bool IsLastSavePoint;
    private bool _hasHealed;

    public void Awake()
    {
        Collider = GameObject.AddComponent<BoxCollider>();
        Collider.Type = ColliderType.Kinematic;

        Trigger = new Trigger();
        Trigger.Transform.LocalScale = GameObject.Transform.LocalScale;
        TriggerScript = Trigger.AddComponent<TriggerScript>();

        GameObject.AddChild(Trigger);
        Trigger.Transform.LocalPosition.Y += 0.045f;

        Scene.Instantiate(Trigger);
    }

    public void OnTriggerEnter(GameObject other)
    {
        var player = other.GetComponent<PlatformerController>();
        if (player == null) return;

        SavePoint = GameObject.Transform.Position;

        if (!_hasHealed)
        {
            player.IncreaseHealth(35f);
            _hasHealed = true;
        }

        if (!IsLastSavePoint) return;
        EOLMenu.IsDrawingEOLMenu = true;
        PlayerHud.StopTimer();
        PauseMenu.UpdatePausedState();
    }
}
