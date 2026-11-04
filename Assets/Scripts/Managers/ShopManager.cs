using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopManager : MonoBehaviour
{
    private readonly string[] _offerIds = { "LifePotion", "Beaf", "Shrimp", "TeaLeaf", "Fish", "Honey" };
    private Player _player;
    private PlayerInventory _inventory;
    private Canvas _canvas;
    private GameObject _panel;
    private TMP_Text _goldText;
    private TMP_Text _statusText;
    private bool _isOpen;
    private InventoryUI _inventoryUI;
    private CharacterPanelUI _characterPanel;

    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _inventory = _player == null ? null : _player.GetComponent<PlayerInventory>();
        _inventoryUI = FindObjectOfType<InventoryUI>();
        _characterPanel = FindObjectOfType<CharacterPanelUI>();
        _canvas = FindObjectOfType<Canvas>();
        Build();
    }

    private void Update()
    {
        if (DialogueManager.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_isOpen) Close();
            else if (NearShopkeeper())
            {
                _inventoryUI?.Close();
                Open();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _isOpen)
        {
            Close();
        }
        if (_isOpen) Refresh();
    }

    public void Open()
    {
        if (_panel == null || _player == null) return;
        if (_inventoryUI == null) _inventoryUI = FindObjectOfType<InventoryUI>();
        if (_characterPanel == null) _characterPanel = FindObjectOfType<CharacterPanelUI>();
        _inventoryUI?.Close();
        _characterPanel?.Close();
        _isOpen = true;
        _panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        _isOpen = false;
        if (_panel != null) _panel.SetActive(false);
    }

    public bool Buy(string itemId)
    {
        if (_player == null || _player.Stats == null || _inventory == null) return false;
        InventoryItemStack item = PlayerInventory.CreateItem(itemId, 1);
        if (item == null || !_player.Stats.SpendGold(item.price)) return false;
        if (!_inventory.AddItem(item))
        {
            _player.Stats.AddGold(item.price);
            return false;
        }
        _statusText.text = "Purchased " + item.displayName;
        Refresh();
        return true;
    }

    private bool NearShopkeeper()
    {
        GameObject shopkeeper = GameObject.Find("NPC_VillageGuard");
        return shopkeeper != null && _player != null &&
               Vector2.Distance(shopkeeper.transform.position, _player.transform.position) <= 2.4f;
    }

    private void Build()
    {
        if (_canvas == null || _panel != null) return;
        _panel = new GameObject("ShopPanel", typeof(RectTransform), typeof(Image));
        _panel.transform.SetParent(_canvas.transform, false);
        RectTransform panelRect = (RectTransform)_panel.transform;
        panelRect.anchorMin = new Vector2(0.54f, 0.14f);
        panelRect.anchorMax = new Vector2(0.94f, 0.9f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        Image background = _panel.GetComponent<Image>();
        background.color = new Color(0.06f, 0.07f, 0.11f, 0.97f);

        TMP_Text title = CreateText("Title", _panel.transform, new Vector2(0.06f, 0.91f), new Vector2(0.7f, 0.99f), 26f);
        title.text = "SHOP  [B / Esc]";
        title.color = new Color(1f, 0.8f, 0.42f);
        _goldText = CreateText("Gold", _panel.transform, new Vector2(0.66f, 0.91f), new Vector2(0.94f, 0.99f), 19f);
        _goldText.alignment = TextAlignmentOptions.Right;
        _goldText.color = new Color(1f, 0.8f, 0.42f);
        _statusText = CreateText("Status", _panel.transform, new Vector2(0.06f, 0.02f), new Vector2(0.94f, 0.09f), 14f);
        _statusText.alignment = TextAlignmentOptions.Center;
        _statusText.color = Color.white;

        for (int i = 0; i < _offerIds.Length; i++) CreateOfferRow(_offerIds[i], i);
        _panel.SetActive(false);
    }

    private void CreateOfferRow(string itemId, int index)
    {
        InventoryItemStack item = PlayerInventory.CreateItem(itemId);
        if (item == null) return;
        float top = 0.84f - index * 0.125f;
        GameObject row = new GameObject("Offer_" + itemId, typeof(RectTransform), typeof(Image));
        row.transform.SetParent(_panel.transform, false);
        RectTransform rowRect = (RectTransform)row.transform;
        rowRect.anchorMin = new Vector2(0.04f, top - 0.105f);
        rowRect.anchorMax = new Vector2(0.96f, top);
        rowRect.offsetMin = Vector2.zero;
        rowRect.offsetMax = Vector2.zero;
        row.GetComponent<Image>().color = new Color(0.13f, 0.15f, 0.2f, 1f);

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(row.transform, false);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0.03f, 0.12f);
        iconRect.anchorMax = new Vector2(0.19f, 0.88f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = item.icon;
        icon.preserveAspect = true;

        TMP_Text name = CreateText("Name", row.transform, new Vector2(0.23f, 0.42f), new Vector2(0.65f, 0.9f), 17f);
        name.text = item.displayName;
        TMP_Text price = CreateText("Price", row.transform, new Vector2(0.23f, 0.08f), new Vector2(0.65f, 0.42f), 14f);
        price.text = item.price + " gold";
        price.color = new Color(1f, 0.8f, 0.42f);
        GameObject buttonObject = new GameObject("Buy", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(row.transform, false);
        RectTransform buttonRect = (RectTransform)buttonObject.transform;
        buttonRect.anchorMin = new Vector2(0.72f, 0.18f);
        buttonRect.anchorMax = new Vector2(0.96f, 0.82f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        buttonObject.GetComponent<Image>().color = new Color(0.2f, 0.55f, 0.36f, 1f);
        string offerId = itemId;
        buttonObject.GetComponent<Button>().onClick.AddListener(() => Buy(offerId));
        TMP_Text buttonText = CreateText("Label", buttonObject.transform, Vector2.zero, Vector2.one, 15f);
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.text = "BUY";
    }

    private void Refresh()
    {
        if (_player == null || _player.Stats == null || _goldText == null) return;
        _goldText.text = _player.Stats.Gold + " gold";
    }

    private static TMP_Text CreateText(string name, Transform parent, Vector2 min, Vector2 max, float size)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)textObject.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.fontSize = size;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }
}
