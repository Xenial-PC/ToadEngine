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
            if (PauseMenu.IsPaused) return;
            Camera!.Update(WHandler.KeyboardState, WHandler.MouseState, deltaTime);
        }

        public override void Resize(FramebufferResizeEventArgs e)
        {
            base.Resize(e);
            Camera!.AspectRatio = WHandler.Size.X / (float)WHandler.Size.Y;
        }
    }

    public class FPControllerScript : Behavior
    {
        private FPCamera _fpCamera = null!;
        private PlayerHud _playerHud = null!;

        public float WalkSpeed = 2.5f, RunSpeed = 10.2f, JumpHeight = 8f;
        public float Acceleration = 45f,
            DeAcceleration = 100f;

        private BoxCollider _collider = null!;
        private Simulation _simulation = null!;
        public BodyReference Body;

        public float StepInterval = 0.45f;
        public float RunStepInterval = 0.33f;

        public float JumpStamina = 100f, JumpStaminaMax = 100f,
            Boost = 100f, BoostMax = 100f,
            Health = 100f, HealthMax = 100f, 
            Speed, MaxSpeed = 20f, HealthDecreaseSpeed = 15.5f;

        private bool _wasGrounded;
        private bool _isWalking, _isRunning, _isAbleToAddSpeed;

        private float _timer;
        public bool IsRespawned = true;

        private float _airSpeedBonus, _groundTime;
        private const float AirSpeedGainRate = 2f;
        private const float MaxAirSpeedBonus = 3.5f;

        public override void Setup()
        {
            _fpCamera = GameObject.GetComponent<FPCamera>("fpCamera");
            _playerHud = GameObject.GetComponent<PlayerHud>()!;

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

            _playerHud.UpdateStaminaUI(JumpStamina / 100f);
            _playerHud.UpdateBoostUI(Boost / 100f);
            _playerHud.UpdateHealthUI(Health / 100f);
        }

        public override void Update(float deltaTime)
        {
            if (PauseMenu.IsPaused) return;
            HandleMove(deltaTime);
            HandleHealth();
        }

        private void HandleHealth()
        {
            if (Health <= 0)
            {
                EOLMenu.IsDrawingLoseScreen = true;
                PlayerHud.StopTimer();
                PauseMenu.UpdatePausedState();
                return;
            }
            DecreaseHealth(HealthDecreaseSpeed);
        }

        private void HandleMove(float deltaTime)
        {
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

            if (isGrounded && !_wasGrounded)
            {
                playerSource.Play(GetSound("land"));
                IncreaseJumpStamina(65f);
                AddBoost(45f);
            }

            if (Input.IsKeyDown(Keys.W)) moveDir += camForward;
            if (Input.IsKeyDown(Keys.S)) moveDir -= camForward;
            if (Input.IsKeyDown(Keys.A)) moveDir += camRight;
            if (Input.IsKeyDown(Keys.D)) moveDir -= camRight;
            if (Input.IsKeyPressed(Keys.Space) && JumpStamina > 0)
            {
                PlayerHud.StartTimer();
                DecreaseJumpStamina(75f);
                ResetTimer(0.5f);

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

            if (!isGrounded && _isRunning)
                DecreaseBoost(85f);

            var velocity = Body.Velocity.Linear;
            var horizontalVel = velocity with { Y = 0 };

            HandleAirSpeed(deltaTime, isGrounded);

            var baseSpeed = _isRunning
                ? (Boost > 0 ? RunSpeed : WalkSpeed)
                : WalkSpeed;

            Speed = baseSpeed + _airSpeedBonus;
            Speed = MathF.Min(Speed, MaxSpeed);

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

        private void HandleAirSpeed(float deltaTime, bool isGrounded)
        {
            if (IsRespawned) _isAbleToAddSpeed = isGrounded;
            if (_isAbleToAddSpeed && IsRespawned)
            {
                Body.Velocity.Linear = new System.Numerics.Vector3(0f);
                Speed = 0;
                IsRespawned = false;
            }

            switch (isGrounded)
            {
                case true:
                    _groundTime += deltaTime;
                    break;
                case false when _isAbleToAddSpeed:
                    _groundTime = 0f;
                    break;
            }

            if (!isGrounded && _isAbleToAddSpeed)
            {
                _airSpeedBonus += AirSpeedGainRate * deltaTime;
                _airSpeedBonus = MathF.Min(_airSpeedBonus, MaxAirSpeedBonus);
            }
            else if (_groundTime >= 0.5f) _airSpeedBonus = 0f;
        }

        private void RegenPlayerJumpStamina()
        {
            if (_timer > 0f) _timer -= 0.5f * DeltaTime;
            if (!(_timer <= 0) || !(JumpStamina < JumpStaminaMax)) return;

            JumpStamina += (!IsInAir() ? 45f : 15f) * DeltaTime;
            _playerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void SetJumpStamina(float stamina)
        {
            JumpStamina = stamina;
            _playerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void IncreaseJumpStamina(float value)
        {
            JumpStamina += value;
            _playerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void DecreaseJumpStamina(float value)
        {
            JumpStamina -= value;
            _playerHud.UpdateStaminaUI(JumpStamina / 100);
        }

        public void SetBoost(float stamina)
        {
            Boost = stamina;
            _playerHud.UpdateBoostUI(Boost / 100);
        }

        public void AddBoost(float value)
        {
            Boost += value;
            if (Boost >= BoostMax) Boost = BoostMax;
            _playerHud.UpdateBoostUI(Boost / 100);
        }

        public void DecreaseBoost(float value)
        {
            Boost -= value * DeltaTime;
            if (Boost <= 0f) Boost = 0f;
            _playerHud.UpdateBoostUI(Boost / 100);
        }

        public void SetHealth(float value)
        {
            Health = value;
            _playerHud.UpdateHealthUI(Health / 100);
        }

        public void IncreaseHealth(float value)
        {
            Health += value;
            if (Health >= HealthMax) Health = HealthMax;
            _playerHud.UpdateHealthUI(Health / 100);
        }

        public void DecreaseHealth(float value)
        {
            Health -= value * DeltaTime;
            if (Health <= 0) Health = 0f;
            _playerHud.UpdateHealthUI(Health / 100);
        }

        private void ResetTimer(float amount)
        {
            _timer = amount;
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
