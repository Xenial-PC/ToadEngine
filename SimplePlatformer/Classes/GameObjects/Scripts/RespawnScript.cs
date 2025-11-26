using SimplePlatformer.Classes.GameObjects.Controllers;
using Quaternion = System.Numerics.Quaternion;

namespace SimplePlatformer.Classes.GameObjects.Scripts;

public class RespawnScript : Behavior
{
    public FPController Player;
    public Vector3 RespawnPosition;

    public override void OnTriggerEnter(GameObject other)
    {
        base.OnTriggerEnter(other);
        if (other.GetComponent<FPController.FPControllerScript>() == null) return;
        
        Player.Controller.Body.Pose.Position = new System.Numerics.Vector3(RespawnPosition.X, RespawnPosition.Y + 14f, RespawnPosition.Z);
        Player.GameObject.Camera!.Transform.LocalRotation = new Vector3(0, 90, 0);
        Player.Controller.SetPlayerJumpStamina(Player.Controller.JumpStaminaMax);
    }

    public override void OnTriggerExit(GameObject other)
    {
        base.OnTriggerExit(other);
    }
}
