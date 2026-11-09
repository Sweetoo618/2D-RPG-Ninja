using System;
using UnityEngine;

/// <summary>单条战利品：掉落物品与数量。</summary>
[Serializable]
public struct LootEntry
{
    [Tooltip("掉落物品（Food / Potion）")]
    public Consumable item;

    [Min(1)]
    public int quantity;
}