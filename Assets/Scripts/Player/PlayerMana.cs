using System;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;


    public float CurrentMana { get; private set; }

    private void Start()
    {
        // Scene loads and save restores update the shared stats asset before Start.
        // Mirror that value instead of resetting it to the maximum every scene.
        if (stats == null) return;
        stats.EnsureDefaults();
        CurrentMana = Mathf.Clamp(stats.Mana, 0f, stats.MaxMana);
        stats.Mana = CurrentMana;
    }

    public void UseMana(float amount)
    {
        if (stats == null || amount <= 0f) return;
        stats.Mana = Mathf.Max(stats.Mana - amount, 0f);
        CurrentMana = stats.Mana;
    }

    public void ResetMana()
    {
        if (stats == null) return;
        stats.Mana = stats.MaxMana;
        CurrentMana = stats.Mana;
    }
}
