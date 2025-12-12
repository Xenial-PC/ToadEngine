using SimplePlatformer.Classes.GameObjects.Menus;
using ToadEngine.Classes.Base.Scripting.Base;
using Vector3 = System.Numerics.Vector3;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class PlatformerController : Behavior
{
    private PlayerHud _playerHud = null!;
    private FPController _controller = null!;

    public float JumpStamina = 100f, JumpStaminaMax = 100f,
        Boost = 100f, BoostMax = 100f,
        Health = 100f, HealthMax = 100f, 
        HealthDecreaseSpeed = 1.5f;

    private float _airSpeedBonus, _groundTime, _healthRegenTimer;
    private const float AirSpeedGainRate = 2f;
    private const float MaxAirSpeedBonus = 3.5f;
    private const float MaxFallingGravity = 1.5f;

    private float _gravityModifier = 1.0f;

    public bool IsRespawned = true;
    private bool _isAbleToAddSpeed;

    public void Awake()
    {
        _controller = GameObject.GetComponent<FPController>()!;
        _controller.OverrideBaseMovement = true;

        _playerHud = _controller.PlayerHud;
    }

    public void Start()
    {
        _playerHud.UpdateStaminaUI(JumpStamina / 100f);
        _playerHud.UpdateBoostUI(Boost / 100f);
        _playerHud.UpdateHealthUI(Health / 100f);
    }

    public void Update()
    {
        _controller.IsAbleToMove = !PauseMenu.IsPaused;
        if (!_controller.IsAbleToMove) return;

        HandleHealth();
        HandleMovement();
    }

    public void HandleMovement()
    {
        _controller.IsAllowedToJump = JumpStamina > 0;
        if (_controller.HasHitGround)
        {
            IncreaseJumpStamina(75f);
            AddBoost(50f);
            _controller.Collider.PhysicsMaterial.Gravity = _gravityModifier = 1.0f;
        }

        if (Input.IsKeyPressed(Keys.Space))
        {
            PlayerHud.StartTimer();

            DecreaseJumpStamina(75f);
            ResetHealthTimer(0.5f);
        }

        if (_controller is { IsGrounded: false, IsRunning: true })
            DecreaseBoost(85f);

        HandleAirSpeed(_controller.IsGrounded);

        var baseSpeed = _controller.IsRunning
            ? (Boost > 0 ? _controller.RunSpeed : _controller.WalkSpeed)
            : _controller.WalkSpeed;

        _controller.Speed = baseSpeed + _airSpeedBonus;
        _controller.Speed = MathF.Min(_controller.Speed, _controller.MaxSpeed);
        
        RegenPlayerJumpStamina();
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

        if (!PlayerHud.LevelTimer.Enabled) return;
        DecreaseHealth(HealthDecreaseSpeed);
    }

    private void HandleAirSpeed(bool isGrounded)
    {
        if (IsRespawned) _isAbleToAddSpeed = isGrounded;
        if (_isAbleToAddSpeed && IsRespawned)
        {
            _controller.Body.Velocity.Linear = new Vector3(0f);
            _controller.Speed = 0;
            IsRespawned = false;
        }

        switch (isGrounded)
        {
            case true:
                _groundTime += Time.DeltaTime;
                break;
            case false when _isAbleToAddSpeed:
                _groundTime = 0f;
                break;
        }

        if (!isGrounded && _isAbleToAddSpeed)
        {
            _airSpeedBonus += AirSpeedGainRate * Time.DeltaTime;
            _airSpeedBonus = MathF.Min(_airSpeedBonus, MaxAirSpeedBonus);

            _gravityModifier += 0.3f * Time.DeltaTime;
            _controller.Collider.PhysicsMaterial.Gravity = _gravityModifier;

            if (_gravityModifier >= MaxFallingGravity) _controller.Collider.PhysicsMaterial.Gravity = MaxFallingGravity;
        }
        else if (_groundTime >= 0.5f) _airSpeedBonus = 0f;
    }

    private void RegenPlayerJumpStamina()
    {
        if (_healthRegenTimer > 0f) _healthRegenTimer -= 0.5f * Time.DeltaTime;
        if (!(_healthRegenTimer <= 0) || !(JumpStamina < JumpStaminaMax)) return;

        var landedStamina = 45f;
        var airStamina = 15f;

        JumpStamina += (!_controller.IsInAir() ? landedStamina : airStamina) * Time.DeltaTime;
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
        if (JumpStamina >= JumpStaminaMax) JumpStamina = JumpStaminaMax;
        _playerHud.UpdateStaminaUI(JumpStamina / 100);
    }

    public void DecreaseJumpStamina(float value)
    {
        JumpStamina -= value;
        if (JumpStamina <= 0) JumpStamina = 0;
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
        Boost -= value * Time.DeltaTime;
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
        Health -= value * Time.DeltaTime;
        if (Health <= 0) Health = 0f;
        _playerHud.UpdateHealthUI(Health / 100);
    }

    private void ResetHealthTimer(float amount) => _healthRegenTimer = amount;
}
