using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战利品面板：打开时根据 LootManager 掉落表实例化物品行，点击 Exit 关闭。
/// 注意：面板初始为 inactive，Awake 可能不执行，因此 Open 中会再次确保接线。
/// </summary>
public sealed class LootPanelUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform content;
    [SerializeField] private LootItemContent itemPrefab;
    [SerializeField] private Button exitButton;
    [SerializeField] private PlayerInventory inventory;

    private readonly List<LootItemContent> _spawned = new List<LootItemContent>();

    private void Awake()
    {
        if (panel == null) panel = gameObject;
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        WireExitButton();
    }

    public void Open()
    {
        if (panel == null) panel = gameObject;
        if (panel.activeSelf) return; // 已打开：不要重复填充（防止点击物品时被阴影点击重置）

        ClearContent();

        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        WireExitButton();

        panel.SetActive(true);

        if (LootManager.Instance == null || itemPrefab == null || content == null) return;

        foreach (Consumable consumable in LootManager.Instance.LootTable)
        {
            if (consumable == null) continue;
            LootItemContent entry = Instantiate(itemPrefab, content);
            entry.Initialize(consumable, LootManager.Instance.LootQuantity, inventory);
            _spawned.Add(entry);
        }
    }

    public void Close()
    {
        ClearContent();
        if (panel != null) panel.SetActive(false);
    }

    // 幂等接线：先移除再添加，避免重复，也保证 Awake 未执行时仍然生效。
    private void WireExitButton()
    {
        if (exitButton == null) return;
        exitButton.onClick.RemoveListener(Close);
        exitButton.onClick.AddListener(Close);
    }

    private void ClearContent()
    {
        if (content != null)
        {
            for (int i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
        _spawned.Clear();
    }
}