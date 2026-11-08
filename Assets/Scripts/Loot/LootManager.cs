using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战利品管理器：开发阶段统一掉落表（所有敌人掉落相同）。
/// 挂在场景的 Managers 物体上，由 LootShadow / LootPanelUI 读取。
/// </summary>
public sealed class LootManager : MonoBehaviour
{
    private static LootManager _instance;

    // 惰性单例：即使 Awake 时序/场景加载顺序异常也能取到。
    public static LootManager Instance
    {
        get
        {
            if (_instance == null)
            {
                LootManager[] found = UnityEngine.Object.FindObjectsOfType<LootManager>(true);
                if (found.Length > 0) _instance = found[0];
            }
            return _instance;
        }
    }

    [Header("Dev Loot Table")]
    [SerializeField] private Consumable[] lootTable;
    [SerializeField] private int lootQuantity = 15;

    [Header("Loot Shadow")]
    [SerializeField] private Sprite shadowSprite;

    public IReadOnlyList<Consumable> LootTable => lootTable;
    public int LootQuantity => Mathf.Max(1, lootQuantity);
    public Sprite ShadowSprite => shadowSprite;

    private void Awake()
    {
        _instance = this;
    }
}