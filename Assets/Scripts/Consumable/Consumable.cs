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
    [Min(0f)] public float restoreExp;

    /// <summary>是否存在任何恢复效果（用于「是否可使用」判断）。</summary>
    public bool HasRestore =>
        restoreHealth > 0f || restoreMana > 0f || restoreExp > 0f;

    /// <summary>应用生命、法力和经验恢复效果。</summary>
    public void Apply(PlayerStats stats, PlayerExp playerExp)
    {
        if (stats == null) return;
        stats.RestoreHealth(restoreHealth);
        stats.RestoreMana(restoreMana);
        playerExp?.AddExp(restoreExp);
    }
}
