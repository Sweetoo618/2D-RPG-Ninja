using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats",menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    public enum AttributeType
    {
        Strength,
        Vitality,
        Intelligence,
        Defense
    }

    [Header("Config")]
    public int Level;

    [Header("Health")]
    public float Health;
    public float MaxHealth;

    [Header("Mana")]
    public float Mana;
    public float MaxMana;

    [Header("Exp")] 
    public float CurrentExp;
    public float NextLevelExp;
    public float InitialNextLevelExp;
    [Range(1f, 100f)] public float ExpMultiplier;

    [Header("Attributes")]
    public int AttributePoints;
    public int Strength;
    public int Vitality;
    public int Intelligence;
    public int Defense;

    [Header("Economy")]
    [Min(0)] public int Gold = 1000;

    public float PhysicalDamageBonus => Strength * 2f;
    public float MagicDamageBonus => Intelligence * 2f;
    public float DamageReduction => Mathf.Clamp01(Defense * 0.025f);
    
    public void ResetPlayer()
    {
        Health = MaxHealth;
        Mana = MaxMana;
        Level = 1;
        CurrentExp = 0f;
        NextLevelExp = InitialNextLevelExp;
        AttributePoints = 0;
        Strength = 0;
        Vitality = 0;
        Intelligence = 0;
        Defense = 0;
        Gold = 1000;
    }

    public void EnsureDefaults()
    {
        if (MaxHealth <= 0f) MaxHealth = 10f;
        if (MaxMana <= 0f) MaxMana = 20f;
        if (InitialNextLevelExp <= 0f) InitialNextLevelExp = 100f;
        if (NextLevelExp <= 0f) NextLevelExp = InitialNextLevelExp;
        if (Level <= 0) Level = 1;
        if (Health <= 0f) Health = MaxHealth;
        if (Mana <= 0f) Mana = MaxMana;
    }

    public bool SpendAttributePoint(AttributeType attribute)
    {
        if (AttributePoints <= 0) return false;
        AttributePoints--;
        switch (attribute)
        {
            case AttributeType.Strength: Strength++; break;
            case AttributeType.Vitality:
                Vitality++;
                MaxHealth += 2f;
                Health += 2f;
                break;
            case AttributeType.Intelligence:
                Intelligence++;
                MaxMana += 3f;
                Mana += 3f;
                break;
            case AttributeType.Defense: Defense++; break;
        }
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0f) return;
        Health = Mathf.Min(MaxHealth, Health + amount);
    }

    public void RestoreMana(float amount)
    {
        if (amount <= 0f) return;
        Mana = Mathf.Min(MaxMana, Mana + amount);
    }
}
