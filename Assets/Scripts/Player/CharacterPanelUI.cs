using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class CharacterPanelUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerExp playerExp;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Canvas canvas;

    private GameObject _panel;
    private TMP_Text _statsText;
    private TMP_Text _inventoryText;
    private TMP_Text _pointText;
    private Image _equipmentIcon;
    private TMP_Text _equipmentLabel;
    private InventoryUI _inventoryUI;
    private ShopManager _shopManager;

    public bool IsOpen => _panel != null && _panel.activeSelf;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<Player>();
        if (playerExp == null) playerExp = FindObjectOfType<PlayerExp>();
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        _inventoryUI = FindObjectOfType<InventoryUI>();
        _shopManager = FindObjectOfType<ShopManager>();
        Build();
    }

    private void Update()
    {
        if (_panel == null || DialogueManager.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_panel.activeSelf)
            {
                Close();
            }
            else
            {
                if (_inventoryUI == null) _inventoryUI = FindObjectOfType<InventoryUI>();
                if (_shopManager == null) _shopManager = FindObjectOfType<ShopManager>();
                _inventoryUI?.Close();
                _shopManager?.Close();
                _panel.SetActive(true);
            }
        }
        if (_panel.activeSelf) Refresh();
    }

    public void Close()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    private void Build()
    {
        if (canvas == null) return;
        _panel = new GameObject("CharacterPanel", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        _panel.transform.SetParent(canvas.transform, false);
        RectTransform rect = (RectTransform)_panel.transform;
        rect.anchorMin = new Vector2(0.62f, 0.36f);
        rect.anchorMax = new Vector2(0.98f, 0.94f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        UnityEngine.UI.Image background = _panel.GetComponent<UnityEngine.UI.Image>();
        background.color = new Color(0.08f, 0.09f, 0.12f, 0.94f);
        _statsText = CreateText("Stats", _panel.transform, new Vector2(0.06f, 0.48f), new Vector2(0.94f, 0.94f), 20f);
        _inventoryText = CreateText("Inventory", _panel.transform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.48f), 16f);
        _equipmentIcon = CreateImage("EquipmentSlot", _panel.transform, new Vector2(0.74f, 0.12f), new Vector2(0.92f, 0.30f));
        _equipmentLabel = CreateText("EquipmentLabel", _panel.transform, new Vector2(0.58f, 0.03f), new Vector2(0.94f, 0.12f), 12f);
        _equipmentLabel.alignment = TextAlignmentOptions.Center;
        _pointText = CreateText("AttributeButtons", _panel.transform, new Vector2(0.06f, 0.43f), new Vector2(0.94f, 0.52f), 16f);
        _pointText.text = "Spend attribute points";
        CreateButton("STR +", _panel.transform, 0.06f, 0.34f, PlayerStats.AttributeType.Strength);
        CreateButton("VIT +", _panel.transform, 0.29f, 0.34f, PlayerStats.AttributeType.Vitality);
        CreateButton("INT +", _panel.transform, 0.52f, 0.34f, PlayerStats.AttributeType.Intelligence);
        CreateButton("DEF +", _panel.transform, 0.75f, 0.34f, PlayerStats.AttributeType.Defense);
        _panel.SetActive(false);
    }

    private void Refresh()
    {
        if (player == null || player.Stats == null) return;
        PlayerStats stats = player.Stats;
        _statsText.text = $"CHARACTER  Lv.{stats.Level}\nHP {stats.Health:0}/{stats.MaxHealth:0}   MP {stats.Mana:0}/{stats.MaxMana:0}\nSTR {stats.Strength}   VIT {stats.Vitality}   INT {stats.Intelligence}   DEF {stats.Defense}\nPoints: {stats.AttributePoints}   Gold: {stats.Gold}";
        _inventoryText.text = inventory == null ? "Inventory unavailable" : BuildInventoryText();
        if (_equipmentIcon != null)
        {
            Weapon equipped = inventory == null ? null : inventory.EquippedWeapon;
            _equipmentIcon.sprite = equipped == null ? null : equipped.icon;
            _equipmentIcon.color = equipped == null ? new Color(0.16f, 0.17f, 0.21f, 1f) : Color.white;
            _equipmentLabel.text = equipped == null ? "Weapon slot: empty" : "Equipped: " + equipped.name;
        }
    }

    private string BuildInventoryText()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder("WEAPONS\n");
        for (int i = 0; i < inventory.Weapons.Count; i++)
        {
            Weapon weapon = inventory.Weapons[i];
            result.Append(i + 1).Append(". ").Append(weapon.name)
                .Append(i == inventory.EquippedIndex ? "  [EQUIPPED]" : "")
                .Append("  ").Append(weapon.damage).Append(" DMG\n");
        }
        result.Append("\nEQUIPMENT\nWeapon: ");
        result.Append(inventory.EquippedWeapon == null ? "Empty" : inventory.EquippedWeapon.name);
        result.Append("\n\nITEMS\n");
        for (int i = 0; i < inventory.Items.Count; i++)
            result.Append(inventory.Items[i].displayName).Append(" x").Append(inventory.Items[i].quantity).Append("\n");
        return result.ToString();
    }

    private static TMP_Text CreateText(string name, Transform parent, Vector2 min, Vector2 max, float fontSize)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)obj.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Image CreateImage(string name, Transform parent, Vector2 min, Vector2 max)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)obj.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.16f, 0.17f, 0.21f, 1f);
        image.preserveAspect = true;
        return image;
    }

    private void CreateButton(string label, Transform parent, float x, float y, PlayerStats.AttributeType attribute)
    {
        GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)buttonObject.transform;
        rect.anchorMin = new Vector2(x, y);
        rect.anchorMax = new Vector2(x + 0.19f, y + 0.075f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        buttonObject.GetComponent<UnityEngine.UI.Image>().color = new Color(0.18f, 0.32f, 0.3f, 1f);
        buttonObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            if (player != null && player.Stats != null) player.Stats.SpendAttributePoint(attribute);
        });
        TMP_Text buttonText = CreateText("Label", buttonObject.transform, Vector2.zero, Vector2.one, 14f);
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.text = label;
    }
}
