using BepuPhysics;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class Player : GameObject;

public class FPController : Behavior
{
    public Camera Camera = null!;
    public PlayerHud PlayerHud = null!;
    public PlatformerController Platformer = null!;
    public BoxCollider Collider = null!;
    public PhysicsBody PhysicsBody = null!;
    public BodyReference Body;

    public float WalkSpeed = 2.5f, RunSpeed = 10.2f, JumpHeight = 8f;
    public float Acceleration = 45f,
        DeAcceleration = 100f;

    public float Speed, MaxSpeed = 20f;

    public float StepInterval = 0.45f;
    public float RunStepInterval = 0.33f;

    public bool IsAbleToMove = true, IsWalking, IsRunning, IsAllowedToJump = true,
        IsGrounded, HasHitGround, OverrideBaseMovement;

    private bool _wasGrounded;

    public void Awake()
    {
        AudioManger.LoadSounds(new Dictionary<string, string>()
        {
            { $"{Directory.GetCurrentDirectory()}/Resources/Audio/jump.mp3", "jump" },
            { $"{Directory.GetCurrentDirectory()}/Resources/Audio/land.mp3", "land" },
            { $"{Directory.GetCurrentDirectory()}/Resources/Audio/step.mp3", "step" },
        });

        Sources.Add("movement", new Source());
        Sources.Add("jump", new Source());

        Camera = Service.MainCamera;
        GameObject.AddChild(Camera);

        Collider = GameObject.AddComponent<BoxCollider>();
        Collider.Type = ColliderType.Dynamic;
        Collider.Size = new Vector3(0.3f, 2f, 0.3f);
        GameObject.Transform.LocalScale = Collider.Size;

        PhysicsBody = GameObject.AddComponent<PhysicsBody>();
        GameObject.UsePhysics = true;

        PlayerHud = GameObject.AddComponent<PlayerHud>();
        Platformer = GameObject.AddComponent<PlatformerController>();
    }

    public void Start()
    {
        Camera.Transform.LocalPosition = GameObject.Transform.Position;
        Camera.Transform.LocalRotation = new Vector3(0f, 90f, 0f);

        Body = Physics.Simulation.Bodies.GetBodyReference(Collider.Collider);
        Body.GetDescription(out var desc);

        desc.LocalInertia.InverseInertiaTensor = default;
        desc.LocalInertia.InverseMass = 0.0000000001f;

        Body.Bodies.ApplyDescription(Body, desc);
    }

    public void OnResize(FramebufferResizeEventArgs e)
    {
        Camera.AspectRatio = WHandler.Size.X / (float)WHandler.Size.Y;
    }

    public void Update()
    {
        if (!IsAbleToMove) return;
        HandleMove();

        Camera.Update();
    }

    private void HandleMove()
    {
        var playerSource = GetSource("movement")!;
        var jumpSource = GetSource("jump")!;

        Physics.Simulation.Awakener.AwakenBody(Body);

        var camForward = new Vector3(Camera.Front.X, 0f, Camera.Front.Z);
        var camRight = new Vector3(Camera.Right.X, 0f, Camera.Right.Z);

        camForward.Normalize();
        camRight.Normalize();

        var isInAir = IsInAir();

        IsGrounded = !isInAir;
        var moveDir = Vector3.Zero;

        if (IsGrounded && !_wasGrounded)
        {
            playerSource.Play(GetSound("land"));
            HasHitGround = true;
        }
        else HasHitGround = false;

        if (Input.IsKeyDown(Keys.W)) moveDir += camForward;
        if (Input.IsKeyDown(Keys.S)) moveDir -= camForward;
        if (Input.IsKeyDown(Keys.A)) moveDir += camRight;
        if (Input.IsKeyDown(Keys.D)) moveDir -= camRight;
        if (Input.IsKeyPressed(Keys.Space) && IsAllowedToJump)
        {
            Body.Velocity.Linear.Y += JumpHeight;
            jumpSource.Play(GetSound("jump"));
        }

        IsRunning = Input.IsKeyDown(Keys.LeftShift);
        IsWalking = Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.A) ||
                     Input.IsKeyDown(Keys.D);

        if (IsGrounded && IsWalking)
        {
            var interval = IsRunning ? RunStepInterval : StepInterval;
            playerSource.Play(GetSound("step"), interval, Time.DeltaTime);
        }

        var velocity = Body.Velocity.Linear;
        var horizontalVel = velocity with { Y = 0 };

        if (!OverrideBaseMovement)
        {
            var baseSpeed = IsRunning ? RunSpeed : WalkSpeed;

            Speed = baseSpeed;
            Speed = MathF.Min(Speed, MaxSpeed);
        }

        var targetVel = System.Numerics.Vector3.Zero;
        if (moveDir.LengthSquared > 0)
        {
            moveDir.Normalize();
            targetVel = new System.Numerics.Vector3(moveDir.X, 0, moveDir.Z) * Speed;
        }

        if (targetVel.LengthSquared() > 0)
        {
            var velChange = targetVel - horizontalVel;
            var changeMagnitude = velChange.Length();
            if (changeMagnitude > 0)
            {
                var maxChange = Acceleration * Time.DeltaTime;
                velChange *= MathF.Min(1f, maxChange / changeMagnitude);
            }

            horizontalVel += velChange;
        }
        else
        {
            var speed = horizontalVel.Length();
            if (speed > 0)
            {
                var drop = DeAcceleration * Time.DeltaTime;
                speed = MathF.Max(0, speed - drop);
                horizontalVel = System.Numerics.Vector3.Normalize(horizontalVel) * speed;
            }
        }

        Body.Velocity.Linear = new System.Numerics.Vector3(horizontalVel.X, velocity.Y, horizontalVel.Z);
        if (targetVel.LengthSquared() > 0)
        {
            var lookYaw = MathF.Atan2(camForward.X, camForward.Z);
            GameObject.Transform.LocalRotation = new Vector3(0, MathHelper.RadiansToDegrees(lookYaw), 0);
            Body.Pose.Orientation = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, lookYaw);
        }

        _wasGrounded = IsGrounded;
        Camera.Transform.LocalPosition = GameObject.Transform.Position + new Vector3(0, 0.2f, 0);

        AudioManger.SetListenerData(GameObject.Transform.Position);
        playerSource.SetPosition(GameObject.Transform.Position);
    }

    public bool IsInAir()
    {
        var position = new Vector3(GameObject.Transform.Position.X,
            GameObject.Transform.Position.Y - GameObject.Transform.Scale.Y * .85f,
            GameObject.Transform.Position.Z);

        var ray = SendRay(position, -Vector3.UnitY);
        if (ray.GameObject == null) return true;

        var dist = ray.Hit.Distance;
        return dist is not float.MaxValue && (dist > 0.5f || !ray.Hit.IsHit);
    }
}
