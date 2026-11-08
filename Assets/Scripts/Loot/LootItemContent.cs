using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 战利品面板中的单个物品行（挂在 Loot_Item_Content 预制体上）：
/// 显示消耗品 SO 的图标/名称/描述/数量，点击后收集进背包并移除本行。
/// </summary>
public sealed class LootItemContent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDes;
    [SerializeField] private TMP_Text itemNum;

    private Consumable _consumable;
    private int _quantity;
    private PlayerInventory _inventory;

    private void Awake()
    {
        if (itemImage == null) itemImage = FindChild<Image>("Item_Image");
        if (itemTitle == null) itemTitle = FindChild<TMP_Text>("Item_Title");
        if (itemDes == null) itemDes = FindChild<TMP_Text>("Item_Des");
        if (itemNum == null) itemNum = FindChild<TMP_Text>("Item_Num");
        if (_inventory == null) _inventory = FindObjectOfType<PlayerInventory>();
    }

    public void Initialize(Consumable consumable, int quantity, PlayerInventory inventory)
    {
        _consumable = consumable;
        _quantity = quantity;
        _inventory = inventory;

        if (itemImage != null) itemImage.sprite = consumable == null ? null : consumable.icon;
        if (itemTitle != null) itemTitle.text = consumable == null ? string.Empty : consumable.itemName;
        if (itemDes != null) itemDes.text = consumable == null ? string.Empty : consumable.description;
        if (itemNum != null) itemNum.text = quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button != PointerEventData.InputButton.Left) return;
        if (_inventory == null) _inventory = FindObjectOfType<PlayerInventory>();
        if (_consumable != null && _inventory != null)
            _inventory.AddConsumable(_consumable, _quantity);
        Destroy(gameObject);
    }

    private T FindChild<T>(string name) where T : Component
    {
        Transform child = transform.Find(name);
        return child == null ? null : child.GetComponent<T>();
    }
}