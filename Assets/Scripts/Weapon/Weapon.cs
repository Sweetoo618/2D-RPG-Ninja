using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Melee,
    Ranged,
    Magic
}

[CreateAssetMenu(fileName = "Weapon_")]
public class Weapon : ScriptableObject
{
    [Header("Info")]
    public string weaponName;
    public string description;

    [Header("Config")]
    public Sprite icon;
    public WeaponType weaponType;
    public float damage;
    [Range(0f, 100f)] public float criticalChance;
    [Min(1f)] public float criticalMultiplier = 2f;
    [Min(0)] public int price;
    [Min(1)] public int maxStack = 99;

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public float requiredMana;

    public float CalculateDamage(out bool isCritical)
    {
        isCritical = criticalChance > 0f && Random.value < criticalChance / 100f;
        return damage * (isCritical ? Mathf.Max(criticalMultiplier, 1f) : 1f);
    }
    
}
