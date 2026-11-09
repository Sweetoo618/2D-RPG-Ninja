using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人死亡后留下的可点击「影子」（使用角色阴影贴图 Actor/Shadow）：
/// 左键点击会打开战利品面板，超时自动消失。
/// 点击检测与 SelectionManager 一致：旧输入 Input.GetMouseButtonDown + Physics2D 查询。
/// </summary>
public sealed class LootShadow : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float lifetime = 30f;
    [SerializeField] private float clickRadius = 1f;

    private Camera _cam;
    private Collider2D _collider;
    private LootPanelUI _lootPanel;
    private int _sortingLayerId;
    private int _sortingOrder;
    private readonly List<LootEntry> _loot = new List<LootEntry>();

    private void Awake()
    {
        _cam = Camera.main;
        if (GetComponent<SpriteRenderer>() == null) gameObject.AddComponent<SpriteRenderer>();

        _collider = GetComponent<Collider2D>();
        if (_collider == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = clickRadius;
            _collider = collider;
        }
    }

    public void Initialize(int sortingLayerId, int sortingOrder, LootEntry[] loot)
    {
        _sortingLayerId = sortingLayerId;
        _sortingOrder = sortingOrder;
        _loot.Clear();
        if (loot != null) _loot.AddRange(loot);
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        LootManager manager = LootManager.Instance;
        if (renderer != null && manager != null && manager.ShadowSprite != null)
        {
            renderer.sprite = manager.ShadowSprite;
            renderer.sortingLayerID = _sortingLayerId;
            renderer.sortingOrder = _sortingOrder;
            renderer.color = Color.white;
        }
    }

    private void Update()
    {
        if (_collider == null || !Input.GetMouseButtonDown(0)) return;
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        Vector2 worldPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == _collider)
            {
                ResolvePanel()?.Open(_loot);
                return;
            }
        }
    }

    // 面板初始为 inactive，FindObjectOfType 找不到，需包含 inactive 查找。
    private LootPanelUI ResolvePanel()
    {
        if (_lootPanel != null) return _lootPanel;
        LootPanelUI[] panels = FindObjectsOfType<LootPanelUI>(true);
        if (panels.Length > 0) _lootPanel = panels[0];
        return _lootPanel;
    }
}