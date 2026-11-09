using UnityEngine;

/// <summary>
/// 战利品阴影配置：提供全局阴影贴图；掉落内容由各敌人自行配置。
/// 挂在场景的 Managers 物体上，由 LootShadow 读取。
/// </summary>
public sealed class LootManager : MonoBehaviour
{
    private static LootManager _instance;

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

    [Header("Loot Shadow")]
    [SerializeField] private Sprite shadowSprite;

    public Sprite ShadowSprite => shadowSprite;

    private void Awake()
    {
        _instance = this;
    }
}