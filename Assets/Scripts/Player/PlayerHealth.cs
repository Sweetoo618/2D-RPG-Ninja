using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    private PlayerAnimations _playerAnimations;

    private void Awake()
    {
        _playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Update()
    {
        if (stats != null && stats.Health <= 0f)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(float amount)
    {
        if (stats == null || stats.Health <= 0f) return;
        float reducedAmount = amount * (1f - stats.DamageReduction);
        stats.Health -= reducedAmount;
        DamageManager.Instance?.ShowDamageText(reducedAmount, transform, false);
        if (stats.Health <= 0f)
        {
            stats.Health = 0f;
            PlayerDead();
        }
    }

    private void PlayerDead()
    {
        _playerAnimations.SetDeadAnimation();
    }
}



