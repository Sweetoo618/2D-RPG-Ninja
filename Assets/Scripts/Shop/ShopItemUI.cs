using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text coinNum;
    [SerializeField] private TMP_Text addNum;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button addButton;
    [SerializeField] private Button subButton;

    private PlayerStats _stats;
    private int _price;
    private int _maxQuantity = 1;
    private int _quantity = 1;
    private Func<int, bool> _canBuy;
    private Func<int, bool> _onBuy;

    private void Awake()
    {
        if (itemImage == null) itemImage = transform.Find("Item_Image")?.GetComponent<Image>();
        if (itemTitle == null) itemTitle = FindText("Item_Title");
        if (coinNum == null) coinNum = FindText("Coin_Num");
        if (addNum == null) addNum = FindText("Add_Num");
        if (buyButton == null) buyButton = FindButton("Buy_Button");
        if (addButton == null) addButton = FindButton("Add_Button");
        if (subButton == null) subButton = FindButton("Sub_Button");

        if (buyButton != null) buyButton.onClick.AddListener(Buy);
        if (addButton != null) addButton.onClick.AddListener(AddQuantity);
        if (subButton != null) subButton.onClick.AddListener(SubtractQuantity);
    }

    private void OnDestroy()
    {
        if (buyButton != null) buyButton.onClick.RemoveListener(Buy);
        if (addButton != null) addButton.onClick.RemoveListener(AddQuantity);
        if (subButton != null) subButton.onClick.RemoveListener(SubtractQuantity);
    }

    public void Bind(Consumable consumable, PlayerInventory inventory, PlayerStats stats)
    {
        if (consumable == null) return;
        long capacity = Mathf.Max(1, inventory != null ? inventory.Capacity : 1);
        int maxQuantity = (int)Math.Min((long)consumable.maxStack * capacity, int.MaxValue);
        Bind(consumable.icon, consumable.itemName, consumable.price, maxQuantity, stats,
            amount => inventory != null && inventory.CanAddConsumable(consumable, amount),
            amount => inventory.AddConsumable(consumable, amount));
    }

    public void Bind(Weapon weapon, PlayerInventory inventory, PlayerStats stats)
    {
        if (weapon == null) return;
        Bind(weapon.icon, weapon.weaponName, weapon.price, Mathf.Max(1, weapon.maxStack), stats,
            amount => inventory != null && inventory.CanAddWeapon(weapon, amount),
            amount => inventory.AddWeapon(weapon, amount));
    }

    private void Bind(Sprite icon, string title, int price, int maxQuantity, PlayerStats stats,
        Func<int, bool> canBuy, Func<int, bool> onBuy)
    {
        _stats = stats;
        _price = price;
        _maxQuantity = Mathf.Max(1, maxQuantity);
        _quantity = 1;
        _canBuy = canBuy;
        _onBuy = onBuy;

        if (itemImage != null)
        {
            itemImage.sprite = icon;
            itemImage.preserveAspect = true;
        }
        if (itemTitle != null) itemTitle.text = title ?? string.Empty;
        if (coinNum != null) coinNum.text = price.ToString();

        RefreshQuantity();
    }

    private void AddQuantity()
    {
        _quantity = (int)Math.Min((long)_maxQuantity, (long)_quantity + 1);
        RefreshQuantity();
    }

    private void SubtractQuantity()
    {
        _quantity = Mathf.Max(1, _quantity - 1);
        RefreshQuantity();
    }

    private void Buy()
    {
        if (_onBuy == null || _canBuy == null || _stats == null) return;

        long totalCost = (long)_price * _quantity;
        if (totalCost > int.MaxValue || _stats.Gold < totalCost) return;
        if (!_canBuy(_quantity)) return;
        if (totalCost > 0 && !_stats.SpendGold((int)totalCost)) return;

        if (!_onBuy(_quantity) && totalCost > 0)
            _stats.AddGold((int)totalCost);
    }

    private void RefreshQuantity()
    {
        if (addNum != null) addNum.text = _quantity.ToString();
        if (subButton != null) subButton.interactable = _quantity > 1;
    }

    private TMP_Text FindText(string childName)
    {
        return transform.Find(childName)?.GetComponent<TMP_Text>();
    }

    private Button FindButton(string childName)
    {
        return transform.Find(childName)?.GetComponent<Button>();
    }
}