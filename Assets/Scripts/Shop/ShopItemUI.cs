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

    private Food _food;
    private PlayerInventory _inventory;
    private PlayerStats _stats;
    private int _quantity = 1;

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

    public void Bind(Food food, PlayerInventory inventory, PlayerStats stats)
    {
        _food = food;
        _inventory = inventory;
        _stats = stats;
        _quantity = 1;

        if (itemImage != null)
        {
            itemImage.sprite = food != null ? food.icon : null;
            itemImage.preserveAspect = true;
        }
        if (itemTitle != null) itemTitle.text = food != null ? food.itemName : string.Empty;
        if (coinNum != null) coinNum.text = food != null ? food.price.ToString() : "0";

        RefreshQuantity();
    }

    private void AddQuantity()
    {
        if (_food == null) return;
        long maxQuantity = (long)Mathf.Max(1, _food.maxStack) * Mathf.Max(1, _inventory != null ? _inventory.Capacity : 1);
        _quantity = (int)Math.Min(maxQuantity, (long)_quantity + 1);
        RefreshQuantity();
    }

    private void SubtractQuantity()
    {
        _quantity = Mathf.Max(1, _quantity - 1);
        RefreshQuantity();
    }

    private void Buy()
    {
        if (_food == null || _inventory == null || _stats == null) return;

        long totalCost = (long)_food.price * _quantity;
        if (totalCost > int.MaxValue || _stats.Gold < totalCost) return;
        if (!_inventory.CanAddConsumable(_food, _quantity)) return;
        if (totalCost > 0 && !_stats.SpendGold((int)totalCost)) return;

        if (!_inventory.AddConsumable(_food, _quantity) && totalCost > 0)
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
