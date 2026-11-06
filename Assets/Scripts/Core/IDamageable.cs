using UnityEngine;

public interface IDamageable
{
    bool IsDead { get; }

    void TakeDamage(float amount);

    void TakeDamage(float amount, bool isCritical);
}
