using BepuPhysics;
using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.View;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class FPController
{
    public FPCamera GameObject { get; private set; }
    public FPControllerScript Controller { get; private set; }
    public PlayerHud PlayerHud { get; private set; }

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
        GameObject.AddComponent("fpCamera", GameObject);
    }

    public class FPCamera : GameObject
    {
        public Camera? Camera;

        public override void Setup()
        {
            base.Setup();

            Camera = GetService<Camera>();
            if (Camera is null)
            {
                Camera = new Camera(WHandler.Size.X / (float)WHandler.Size.Y);
                AddService(Camera);
            }

            AddChild(Camera);

            Camera.Transform.LocalPosition = Transform.LocalPosition;
            Camera.Transform.LocalRotation = new Vector3(0f, 90f, 0f);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (PauseMenu.IsPaused) return;
            Camera!.Update(WHandler.KeyboardState, WHandler.MouseState, deltaTime);
        }

        public override void Resize(FramebufferResizeEventArgs e)
        {
            base.Resize(e);
            Camera!.AspectRatio = WHandler.Size.X / (float)WHandler.Size.Y;
        }
    }

    public class FPControllerScript : Behaviour
    {
        private FPCamera? _fpCamera;
        
        public float WalkSpeed = 2.5f, RunSpeed = 4.2f, JumpHeight = 8f;
        public float Acceleration = 30f,
            DeAcceleration = 10f;

        private BoxCollider _collider = null!;
        private Simulation _simulation = null!;
        public BodyReference Body;

        public float StepInterval = 0.45f;
        public float RunStepInterval = 0.33f;

        public float JumpStamina = 100f, JumpStaminaMax = 100f;

        private bool _wasGrounded;
        private bool _isWalking, _isRunning;

        private float _timer;

        public override void Setup()
        {
            base.Setup();
            _fpCamera = GameObject.GetComponent<FPCamera>("fpCamera");

            GameObject.UsePhysics = true;

            _simulation = GetCurrentScene().PhysicsManager.Simulation;
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

            PlayerHud.UpdateStaminaUI(JumpStamina / 100f);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            HandleMove(deltaTime);
        }

        private void HandleMove(float deltaTime)
        {
            if (PauseMenu.IsPaused) return;

            var playerSource = GetSource("movement")!;
            var jumpSource = GetSource("jump")!;

            _simulation.Awakener.AwakenBody(Body);

            var camForward = new Vector3(_fpCamera!.Camera!.Front.X, 0f, _fpCamera.Camera.Front.Z);
            var camRight = new Vector3(_fpCamera.Camera.Right.X, 0f, _fpCamera.Camera.Right.Z);

            camForward.Normalize();
            camRight.Normalize();

            var isInAir = IsInAir();

            var isGrounded = !isInAir;
            var moveDir = Vector3.Zero;

            if (isGrounded && !_wasGrounded) playerSource.Play(GetSound("land"));

            if (Input.IsKeyDown(Keys.W)) moveDir += camForward;
            if (Input.IsKeyDown(Keys.S)) moveDir -= camForward;
            if (Input.IsKeyDown(Keys.A)) moveDir += camRight;
            if (Input.IsKeyDown(Keys.D)) moveDir -= camRight;
            if (Input.IsKeyPressed(Keys.Space) && JumpStamina > 0)
            {
                DecreasePlayerJumpStamina(20f);
                ResetTimer(1.5f);

                Body.Velocity.Linear.Y += JumpHeight;
                jumpSource.Play(GetSound("jump"));
            }

            RegenPlayerJumpStamina();

            _isRunning = Input.IsKeyDown(Keys.LeftShift);
            _isWalking = Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.A) ||
                         Input.IsKeyDown(Keys.D);

            if (isGrounded && _isWalking)
            {
                var interval = _isRunning ? RunStepInterval : StepInterval;
                playerSource.Play(GetSound("step"), interval, deltaTime);
            }

            var velocity = Body.Velocity.Linear;
            var horizontalVel = new System.Numerics.Vector3(velocity.X, 0, velocity.Z);
            var maxSpeed = Input.IsKeyDown(Keys.LeftShift) ? RunSpeed : WalkSpeed;

            var targetVel = System.Numerics.Vector3.Zero;
            if (moveDir.LengthSquared > 0)
            {
                moveDir.Normalize();
                targetVel = new System.Numerics.Vector3(moveDir.X, 0, moveDir.Z) * maxSpeed;
            }

            if (targetVel.LengthSquared() > 0)
            {
                var velChange = targetVel - horizontalVel;
                var changeMagnitude = velChange.Length();
                if (changeMagnitude > 0)
                {
                    var maxChange = Acceleration * deltaTime;
                    velChange *= MathF.Min(1f, maxChange / changeMagnitude);
                }

                horizontalVel += velChange;
            }
            else
            {
                var speed = horizontalVel.Length();
                if (speed > 0)
                {
                    var drop = DeAcceleration * deltaTime;
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

            _wasGrounded = isGrounded;
            _fpCamera.Camera.Transform.LocalPosition = GameObject.Transform.Position + new Vector3(0, 0.2f, 0);

            AudioManger.SetListenerData(GameObject.Transform.Position);
            playerSource.SetPosition(GameObject.Transform.Position);
        }

        private void RegenPlayerJumpStamina()
        {
            if (_timer > 0f) _timer -= 0.5f * DeltaTime;
            if (!(_timer <= 0) || !(JumpStamina < JumpStaminaMax)) return;

            JumpStamina += 15f * DeltaTime;
            PlayerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        private void ResetTimer(float amount)
        {
            _timer = amount;
        }

        public void SetPlayerJumpStamina(float stamina)
        {
            JumpStamina = stamina;
            PlayerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void IncreasePlayerJumpStamina(float value)
        {
            JumpStamina += value;
            PlayerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void DecreasePlayerJumpStamina(float value)
        {
            JumpStamina -= value;
            PlayerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        private bool IsInAir()
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
