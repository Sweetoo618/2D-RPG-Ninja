using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerDeathPanel deathPanel;

    private PlayerAnimations _playerAnimations;
    private PlayerMovement _playerMovement;
    private PlayerAttack _playerAttack;
    private bool _deathHandled;

    public bool IsDead => stats == null || stats.Health <= 0f;

    private void Awake()
    {
        _playerAnimations = GetComponent<PlayerAnimations>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (IsDead)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, false);
    }

    public void TakeDamage(float amount, bool isCritical)
    {
        if (stats == null || IsDead) return;
        float reducedAmount = amount * (1f - stats.DamageReduction);
        stats.Health -= reducedAmount;
        DamageManager.Instance?.ShowDamageText(reducedAmount, transform, isCritical);
        if (IsDead)
        {
            stats.Health = 0f;
            PlayerDead();
        }
    }

    private void PlayerDead()
    {
        _playerAnimations.SetDeadAnimation();
        if (_deathHandled) return;
        _deathHandled = true;
        SfxManager.Instance?.PlayFailed();
        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerAttack != null) _playerAttack.enabled = false;
        SaveSystem.DeleteSave();
        if (deathPanel != null) deathPanel.Show();
    }
}