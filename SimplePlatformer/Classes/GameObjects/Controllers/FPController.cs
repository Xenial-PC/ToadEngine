using BepuPhysics;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class FPController
{
    public FPCamera GameObject { get; private set; }
    public PlayerHud PlayerHud { get; private set; }

    public FPControllerScript Controller { get; private set; }
    public PlatformerController PController { get; private set; }

    public FPController(Vector3 size, float mass = 1f)
    {
        Load(size, mass);
    }

    public void Load(Vector3 size, float mass)
    {
        GameObject = new FPCamera();
        GameObject.Transform.LocalScale = size;

        PlayerHud = GameObject.AddComponent<PlayerHud>();
        GameObject.AddComponent<PhysicsBody>();

        var collider = GameObject.AddComponent<BoxCollider>();
        collider.Type = ColliderType.Dynamic;
        collider.Size = size;

        Controller = GameObject.AddComponent<FPControllerScript>();
        PController = GameObject.AddComponent<PlatformerController>();

        GameObject.AddComponent("fpCamera", GameObject);
    }

    public class FPCamera : GameObject
    {
        public Camera? Camera;

        public override void Setup()
        {
            Camera = Service.MainCamera;
            if (Camera is null)
            {
                Camera = new Camera();
                Service.MainCamera = Camera;
            }

            AddChild(Camera);

            Camera.Transform.LocalPosition = Transform.LocalPosition;
            Camera.Transform.LocalRotation = new Vector3(0f, 90f, 0f);
        }
    }

    public class FPControllerScript : Behavior
    {
        private FPCamera _fpCamera = null!;
        
        public float WalkSpeed = 2.5f, RunSpeed = 10.2f, JumpHeight = 8f;
        public float Acceleration = 45f,
            DeAcceleration = 100f;

        public float Speed, MaxSpeed = 20f;

        private BoxCollider _collider = null!;
        private Simulation _simulation = null!;
        public BodyReference Body;

        public float StepInterval = 0.45f;
        public float RunStepInterval = 0.33f;

        public bool IsAbleToMove = true, IsWalking, IsRunning, IsAllowedToJump = true,
            IsGrounded, HasHitGround, OverrideBaseMovement;

        private bool _wasGrounded;

        public override void OnStart()
        {
            _fpCamera = GameObject.GetComponent<FPCamera>("fpCamera");
            GameObject.UsePhysics = true;

            _simulation = Physics.Simulation;
            _collider = GameObject.GetComponent<BoxCollider>()!;

            Body = _simulation.Bodies.GetBodyReference(_collider.Collider);
            Body.GetDescription(out var desc);

            desc.LocalInertia.InverseInertiaTensor = default;
            desc.LocalInertia.InverseMass = 0.0000000001f;

            Body.Bodies.ApplyDescription(Body, desc);

            AudioManger.LoadSounds(new Dictionary<string, string>()
            {
                { $"{Directory.GetCurrentDirectory()}/Resources/Audio/jump.mp3", "jump" },
                { $"{Directory.GetCurrentDirectory()}/Resources/Audio/land.mp3", "land" },
                { $"{Directory.GetCurrentDirectory()}/Resources/Audio/step.mp3", "step" },
            });

            Sources.Add("movement", new Source());
            Sources.Add("jump", new Source());
        }

        public override void OnResize(FramebufferResizeEventArgs e)
        {
            _fpCamera.Camera!.AspectRatio = WHandler.Size.X / (float)WHandler.Size.Y;
        }

        public override void OnUpdate()
        {
            if (!IsAbleToMove) return;
            HandleMove();

            _fpCamera.Camera!.Update();
        }

        private void HandleMove()
        {
            var playerSource = GetSource("movement")!;
            var jumpSource = GetSource("jump")!;

            _simulation.Awakener.AwakenBody(Body);

            var camForward = new Vector3(_fpCamera!.Camera!.Front.X, 0f, _fpCamera.Camera.Front.Z);
            var camRight = new Vector3(_fpCamera.Camera.Right.X, 0f, _fpCamera.Camera.Right.Z);

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
            _fpCamera.Camera.Transform.LocalPosition = GameObject.Transform.Position + new Vector3(0, 0.2f, 0);

            AudioManger.SetListenerData(GameObject.Transform.Position);
            playerSource.SetPosition(GameObject.Transform.Position);
        }

        public bool IsInAir()
        {
            var position = new Vector3(GameObject.Transform.Position.X,
                GameObject.Transform.Position.Y - _fpCamera!.Transform.Scale.Y * .52f,
                GameObject.Transform.Position.Z);

            var ray = SendRay(position, -Vector3.UnitY);
            if (ray.GameObject == null) return true;

            var dist = ray.Hit.Distance;
            if (dist is not float.MaxValue && (dist > 0.5f || !ray.Hit.IsHit)) return true;

            return false;
        }
    }
}
