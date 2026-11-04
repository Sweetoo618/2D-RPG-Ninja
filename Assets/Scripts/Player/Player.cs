using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    public PlayerStats Stats => stats;
    public PlayerMana PlayerMana { get; private set; }
    public PlayerAnimations animations;

    private void Awake()
    {
        PlayerMana = GetComponent<PlayerMana>();
        animations = GetComponent<PlayerAnimations>();
    }

    private void Start()
    {
        stats?.EnsureDefaults();
        // The shared asset was previously saved with zero health, so make sure a new run starts alive.
        if (stats != null && (stats.Health <= 0f || stats.NextLevelExp <= 0f))
        {
            stats.ResetPlayer();
        }
    }

    public void ResetPlayer()
    {
        if (stats == null) return;
        stats.ResetPlayer();
        animations?.ResetPlayer();
        PlayerMana?.ResetMana();
    }
}