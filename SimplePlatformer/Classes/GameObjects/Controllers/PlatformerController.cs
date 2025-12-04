using SimplePlatformer.Classes.GameObjects.Menus;
using SimplePlatformer.Classes.GameObjects.Scripts;
using ToadEngine.Classes.Base.Scripting.Base;

namespace SimplePlatformer.Classes.GameObjects.Controllers;

public class PlatformerController : Behavior
{
    private PlayerHud _playerHud = null!;
    private FPController.FPControllerScript _fpScript = null!;

    public float JumpStamina = 100f, JumpStaminaMax = 100f,
        Boost = 100f, BoostMax = 100f,
        Health = 100f, HealthMax = 100f, 
        HealthDecreaseSpeed = 1.5f;

    private float _airSpeedBonus, _groundTime, _timer;
    private const float AirSpeedGainRate = 2f;
    private const float MaxAirSpeedBonus = 3.5f;

    public bool IsRespawned = true;
    private bool _isAbleToAddSpeed;

    public override void OnStart()
    {
        _fpScript = GameObject.GetComponent<FPController.FPControllerScript>()!;
        _fpScript.OverrideBaseMovement = true;

        _playerHud = GameObject.GetComponent<PlayerHud>()!;

        _playerHud.UpdateStaminaUI(JumpStamina / 100f);
        _playerHud.UpdateBoostUI(Boost / 100f);
        _playerHud.UpdateHealthUI(Health / 100f);
    }

    public override void OnUpdate()
    {
        _fpScript.IsAbleToMove = !PauseMenu.IsPaused;
        if (!_fpScript.IsAbleToMove) return;

        HandleHealth();
        HandleMovement();
    }

    public void HandleMovement()
    {
        _fpScript.IsAllowedToJump = JumpStamina > 0;
        if (_fpScript.HasHitGround)
        {
            IncreaseJumpStamina(75f);
            AddBoost(50f);
        }

        if (Input.IsKeyPressed(Keys.Space))
        {
            PlayerHud.StartTimer();

            DecreaseJumpStamina(75f);
            ResetTimer(0.5f);
        }

        if (_fpScript is { IsGrounded: false, IsRunning: true })
            DecreaseBoost(85f);

        HandleAirSpeed(Time.DeltaTime, _fpScript.IsGrounded);

        var baseSpeed = _fpScript.IsRunning
            ? (Boost > 0 ? _fpScript.RunSpeed : _fpScript.WalkSpeed)
            : _fpScript.WalkSpeed;

        _fpScript.Speed = baseSpeed + _airSpeedBonus;
        _fpScript.Speed = MathF.Min(_fpScript.Speed, _fpScript.MaxSpeed);
        
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
            _fpScript.Body.Velocity.Linear = new System.Numerics.Vector3(0f);
            _fpScript.Speed = 0;
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
        if (_timer > 0f) _timer -= 0.5f * Time.DeltaTime;
        if (!(_timer <= 0) || !(JumpStamina < JumpStaminaMax)) return;

        JumpStamina += (!_fpScript.IsInAir() ? 45f : 15f) * Time.DeltaTime;
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

    private void ResetTimer(float amount)
    {
        _timer = amount;
    }
}
