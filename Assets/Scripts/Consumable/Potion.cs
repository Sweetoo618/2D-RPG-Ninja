using UnityEngine;

/// <summary>
/// 药水类消耗品。字段继承自 Consumable（回血/回蓝、价格、堆叠、图标等）。
/// 与 Food 区分为独立类型，便于后续在药水上扩展增益/时长等专属效果。
/// </summary>
[CreateAssetMenu(fileName = "Potion_", menuName = "Items/Potion")]
public sealed class Potion : Consumable
{
}