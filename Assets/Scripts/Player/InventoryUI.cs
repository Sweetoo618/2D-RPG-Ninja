using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Controls the scene-authored inventory view without creating UI objects.</summary>
public sealed class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image[] slotImages = new Image[24];
    [SerializeField] private TMP_Text[] slotLabels = new TMP_Text[24];
    [SerializeField] private Button[] slotButtons = new Button[24];
    [SerializeField] private TMP_Text summary;
    private PlayerInventory _inventory;
    private ShopManager _shop;
    private CharacterPanelUI _characterPanel;
    private bool _isOpen;
    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _inventory = FindObjectOfType<PlayerInventory>();
        _shop = FindObjectOfType<ShopManager>();
        _characterPanel = FindObjectOfType<CharacterPanelUI>();
        if (panel == null) panel = transform.Find("InventoryPanel")?.gameObject;
        if (slotButtons != null)
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int index = i;
                if (slotButtons[i] != null) slotButtons[i].onClick.AddListener(() => UseSlot(index));
            }
        }
        if (_inventory != null) _inventory.Changed += Refresh;
        Close();
    }

    private void OnDestroy()
    {
        if (_inventory != null) _inventory.Changed -= Refresh;
    }

    private void Update()
    {
        if (DialogueManager.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_shop != null && _shop.IsOpen) return;
            if (_isOpen) Close(); else Open();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _isOpen) Close();
        if (_isOpen) Refresh();
    }

    public void Open()
    {
        if (panel == null) return;
        if (_shop == null) _shop = FindObjectOfType<ShopManager>();
        if (_characterPanel == null) _characterPanel = FindObjectOfType<CharacterPanelUI>();
        if (_shop != null && _shop.IsOpen) return;
        _characterPanel?.Close();
        _isOpen = true;
        panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        _isOpen = false;
        if (panel != null) panel.SetActive(false);
    }

    private void Refresh()
    {
        if (_inventory == null || slotImages == null || slotLabels == null) return;
        int count = Mathf.Min(slotImages.Length, slotLabels.Length);
        for (int i = 0; i < count; i++)
        {
            InventoryItemStack item = i < _inventory.Items.Count ? _inventory.Items[i] : null;
            Image image = slotImages[i];
            TMP_Text label = slotLabels[i];
            if (image != null)
            {
                image.sprite = item == null ? null : item.icon;
                image.color = item == null ? new Color(0.16f, 0.17f, 0.21f, 1f) : Color.white;
                image.type = item == null || item.icon == null ? Image.Type.Simple : Image.Type.Sliced;
            }
            if (label != null) label.text = item == null ? string.Empty : item.quantity.ToString();
        }
        if (summary != null) summary.text = "Click a food or potion to use it";
    }

    public void UseSlot(int index)
    {
        if (_inventory != null) _inventory.UseItem(index);
    }
}
