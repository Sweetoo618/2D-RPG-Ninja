using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    private PlayerAnimations _playerAnimations;
    private bool _deadSoundPlayed;

    public bool IsDead => stats == null || stats.Health <= 0f;

    private void Awake()
    {
        _playerAnimations = GetComponent<PlayerAnimations>();
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
        if (_deadSoundPlayed) return;
        _deadSoundPlayed = true;
        SfxManager.Instance?.PlayFailed();
    }
}