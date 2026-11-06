using UnityEngine;

/// <summary>
/// 食物类消耗品。字段继承自 Consumable（回血/回蓝、价格、堆叠、图标等）。
/// 与 Potion 区分为独立类型，便于在背包/商店/战斗中分别处理与扩展。
/// </summary>
[CreateAssetMenu(fileName = "Food_", menuName = "Items/Food")]
public sealed class Food : Consumable
{
}