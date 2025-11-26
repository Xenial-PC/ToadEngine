using SimplePlatformer.Classes.GameObjects.Controllers;
using SimplePlatformer.Classes.GameObjects.Menus;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class SavePointScript : Behavior
{
    public static Vector3 SavePoint;
    public bool IsLastSavePoint;

    public override void OnTriggerEnter(GameObject other)
    {
        if (other.GetComponent<FPController.FPControllerScript>() == null) return;
        SavePoint = GameObject.Transform.Position;

        if (IsLastSavePoint)
        {
            EOLMenu.IsDrawingEOLMenu = true;
            PauseMenu.UpdatePausedState();
        }
    }
}
