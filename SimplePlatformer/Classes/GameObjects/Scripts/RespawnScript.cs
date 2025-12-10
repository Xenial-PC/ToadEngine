using SimplePlatformer.Classes.GameObjects.Controllers;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class RespawnScript : Behavior
{
    public Player Player = null!;
    public FPController Controller = null!;
    public static Vector3 RespawnPosition;

    public void OnTriggerEnter(GameObject other)
    {
        if (other.GetComponent<FPController>() == null) return;
        
        Controller.Body.Pose.Position = new System.Numerics.Vector3(RespawnPosition.X, RespawnPosition.Y + 14f, RespawnPosition.Z);
        Controller.Camera!.Transform.LocalRotation = new Vector3(0, 90, 0);
        Controller.Platformer.SetJumpStamina(Controller.Platformer.JumpStaminaMax);
        Controller.Platformer.IsRespawned = true;
    }
}
