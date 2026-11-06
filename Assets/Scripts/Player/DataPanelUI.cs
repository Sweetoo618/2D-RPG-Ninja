using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Data-Panel 顶部标签导航：Inventory / Stats / Quest 三个面板互斥显示，
/// 再次点击当前标签则收起（toggle）。按钮与面板引用在 Inspector 中挂接。
/// </summary>
public sealed class DataPanelUI : MonoBehaviour
{
    [Header("Nav Buttons")]
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button questButton;

    [Header("Panels")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject questPanel;

    private void Awake()
    {
        inventoryButton?.onClick.AddListener(OnInventoryClicked);
        statsButton?.onClick.AddListener(OnStatsClicked);
        questButton?.onClick.AddListener(OnQuestClicked);
    }

    private void Start()
    {
        // 初始全部收起，保证同一时间不显示任何面板。
        ShowPanel(-1);
    }

    public void OnInventoryClicked() => Toggle(0);

    public void OnStatsClicked() => Toggle(1);

    public void OnQuestClicked() => Toggle(2);

    private void Toggle(int index)
    {
        if (IsActive(index)) ShowPanel(-1);
        else ShowPanel(index);
    }

    private bool IsActive(int index)
    {
        switch (index)
        {
            case 0: return inventoryUI != null && inventoryUI.IsOpen;
            case 1: return statsPanel != null && statsPanel.activeSelf;
            case 2: return questPanel != null && questPanel.activeSelf;
            default: return false;
        }
    }

    private void ShowPanel(int active)
    {
        if (inventoryUI != null)
        {
            if (active == 0) inventoryUI.Open();
            else inventoryUI.Close();
        }
        if (statsPanel != null) statsPanel.SetActive(active == 1);
        if (questPanel != null) questPanel.SetActive(active == 2);
    }
}