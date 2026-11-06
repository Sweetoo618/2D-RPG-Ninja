using UnityEngine;

/// <summary>
/// 消耗品基类：Food / Potion 共享的数据与效果结算。
/// 抽象类，不直接创建资产；请用 Food 或 Potion 的 CreateAssetMenu 创建。
/// </summary>
public abstract class Consumable : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;

    [Header("Config")]
    public Sprite icon;
    [Min(0)] public int price;
    [Min(1)] public int maxStack = 99;

    [Header("Restore")]
    [Min(0f)] public float restoreHealth;
    [Min(0f)] public float restoreMana;
    [Range(0f, 1f)] public float restoreHealthPercent;
    [Range(0f, 1f)] public float restoreManaPercent;

    /// <summary>是否存在任何恢复效果（用于「是否可使用」判断）。</summary>
    public bool HasRestore =>
        restoreHealth > 0f || restoreMana > 0f ||
        restoreHealthPercent > 0f || restoreManaPercent > 0f;

    /// <summary>实际回血量 = 固定值 + 最大生命 × 百分比。</summary>
    public float GetHealthRestore(float maxHealth) => restoreHealth + maxHealth * restoreHealthPercent;

    /// <summary>实际回蓝量 = 固定值 + 最大法力 × 百分比。</summary>
    public float GetManaRestore(float maxMana) => restoreMana + maxMana * restoreManaPercent;

    /// <summary>应用效果到目标属性（恢复生命/法力，自动夹紧到上限）。</summary>
    public void Apply(PlayerStats stats)
    {
        if (stats == null) return;
        if (restoreHealth > 0f || restoreHealthPercent > 0f)
            stats.RestoreHealth(GetHealthRestore(stats.MaxHealth));
        if (restoreMana > 0f || restoreManaPercent > 0f)
            stats.RestoreMana(GetManaRestore(stats.MaxMana));
    }
}