using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Rendering.Object;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class SavePointScript : Behavior
{
    public static Vector3 SavePoint;
    public bool IsLastSavePoint;
    private bool _hasHealed;

    public override void OnTriggerEnter(GameObject other)
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
