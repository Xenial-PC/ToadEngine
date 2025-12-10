using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Base.Scripting.Base;
using Vector3 = System.Numerics.Vector3;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class PlatformerController : Behavior
{
    private PlayerHud _playerHud = null!;
    private FPController _fp = null!;

    public float JumpStamina = 100f, JumpStaminaMax = 100f,
        Boost = 100f, BoostMax = 100f,
        Health = 100f, HealthMax = 100f, 
        HealthDecreaseSpeed = 1.5f;

    private float _airSpeedBonus, _groundTime, _healthRegenTimer;
    private const float AirSpeedGainRate = 2f;
    private const float MaxAirSpeedBonus = 3.5f;
    private const float MaxFallingGravity = -18;

    public bool IsRespawned = true;
    private bool _isAbleToAddSpeed;

    public void Awake()
    {
        _fp = GameObject.GetComponent<FPController>()!;
        _fp.OverrideBaseMovement = true;

        _playerHud = _fp.PlayerHud;
    }

    public void Start()
    {
        _playerHud.UpdateStaminaUI(JumpStamina / 100f);
        _playerHud.UpdateBoostUI(Boost / 100f);
        _playerHud.UpdateHealthUI(Health / 100f);
    }

    public void Update()
    {
        _fp.IsAbleToMove = !PauseMenu.IsPaused;
        if (!_fp.IsAbleToMove) return;

        HandleHealth();
        HandleMovement();
    }

    public void HandleMovement()
    {
        _fp.IsAllowedToJump = JumpStamina > 0;
        if (_fp.HasHitGround)
        {
            IncreaseJumpStamina(75f);
            AddBoost(50f);
            Physics.Settings.Gravity.Y = -10;
        }

        if (Input.IsKeyPressed(Keys.Space))
        {
            PlayerHud.StartTimer();

            DecreaseJumpStamina(75f);
            ResetHealthTimer(0.5f);
        }

        if (_fp is { IsGrounded: false, IsRunning: true })
            DecreaseBoost(85f);

        HandleAirSpeed(Time.DeltaTime, _fp.IsGrounded);

        var baseSpeed = _fp.IsRunning
            ? (Boost > 0 ? _fp.RunSpeed : _fp.WalkSpeed)
            : _fp.WalkSpeed;

        _fp.Speed = baseSpeed + _airSpeedBonus;
        _fp.Speed = MathF.Min(_fp.Speed, _fp.MaxSpeed);
        
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

    private void HandleAirSpeed(float deltaTime, bool isGrounded)
    {
        if (IsRespawned) _isAbleToAddSpeed = isGrounded;
        if (_isAbleToAddSpeed && IsRespawned)
        {
            _fp.Body.Velocity.Linear = new Vector3(0f);
            _fp.Speed = 0;
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

            Physics.Settings.Gravity.Y += -7.5f * deltaTime;
            if (Physics.Settings.Gravity.Y <= MaxFallingGravity) Physics.Settings.Gravity.Y = MaxFallingGravity;
        }
        else if (_groundTime >= 0.5f) _airSpeedBonus = 0f;
    }

    private void RegenPlayerJumpStamina()
    {
        if (_healthRegenTimer > 0f) _healthRegenTimer -= 0.5f * Time.DeltaTime;
        if (!(_healthRegenTimer <= 0) || !(JumpStamina < JumpStaminaMax)) return;

        var landedStamina = 45f;
        var airStamina = 15f;

        JumpStamina += (!_fp.IsInAir() ? landedStamina : airStamina) * Time.DeltaTime;
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
