using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Drives the scene-authored 4x6 inventory grid.</summary>
public sealed class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private InventoryInfoPanelUI infoPanel;

    private struct SlotContent
    {
        public Sprite icon;
        public int quantity;
        public Weapon weapon;
    }

    private readonly List<InventorySlotUI> _slots = new List<InventorySlotUI>();
    private PlayerInventory _inventory;
    private ShopManager _shop;
    private CharacterPanelUI _characterPanel;
    private SlotContent[] _contents;
    private Canvas _canvas;
    private GameObject _dragGhost;
    private int _draggedIndex = -1;
    private int _hoveredIndex = -1;
    private bool _isOpen;

    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _inventory = FindObjectOfType<PlayerInventory>();
        _shop = FindObjectOfType<ShopManager>();
        _characterPanel = FindObjectOfType<CharacterPanelUI>();
        _canvas = GetComponent<Canvas>();
        if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        if (panel == null) panel = transform.Find("Inventory-Panel")?.gameObject;
        CollectSlots();
        if (_inventory != null) _inventory.Changed += Refresh;
        _isOpen = panel != null && panel.activeSelf;
        Refresh();
        if (infoPanel != null) infoPanel.Hide();
    }

    private void OnDestroy()
    {
        if (_inventory != null) _inventory.Changed -= Refresh;
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
        _hoveredIndex = -1;
        RefreshHoverInfo();
        if (panel != null) panel.SetActive(false);
    }

    public void BeginDrag(int index, Sprite icon, PointerEventData eventData)
    {
        if (_canvas == null || _contents == null || index < 0 || index >= _contents.Length) return;
        if (icon == null) return;
        _draggedIndex = index;
        if (_dragGhost != null) Destroy(_dragGhost);
        _dragGhost = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _dragGhost.transform.SetParent(_canvas.transform, false);
        RectTransform rect = (RectTransform)_dragGhost.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(70f, 70f);
        Image image = _dragGhost.GetComponent<Image>();
        image.sprite = icon;
        image.preserveAspect = true;
        image.raycastTarget = false;
        UpdateDrag(eventData);
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (_dragGhost == null || _canvas == null) return;
        RectTransform canvasRect = (RectTransform)_canvas.transform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position,
                eventData.pressEventCamera, out Vector2 localPoint))
        {
            ((RectTransform)_dragGhost.transform).anchoredPosition = localPoint;
        }
    }

    public void HoverSlot(int index)
    {
        _hoveredIndex = index;
        RefreshHoverInfo();
    }

    public void ClearHover(int index)
    {
        if (_hoveredIndex != index) return;
        _hoveredIndex = -1;
        RefreshHoverInfo();
    }

    public void DropOn(int targetIndex)
    {
        if (_draggedIndex < 0 || _contents == null) return;
        if (targetIndex < 0 || targetIndex >= _contents.Length || _draggedIndex >= _contents.Length) return;
        if (_draggedIndex != targetIndex)
        {
            SlotContent temp = _contents[_draggedIndex];
            _contents[_draggedIndex] = _contents[targetIndex];
            _contents[targetIndex] = temp;
        }
        _draggedIndex = -1;
        ApplyContents();
        RefreshHoverInfo();
    }

    public void DropOnEquip()
    {
        if (_inventory == null || _contents == null) return;
        if (_draggedIndex < 0 || _draggedIndex >= _contents.Length) return;
        Weapon weapon = _contents[_draggedIndex].weapon;
        if (weapon != null) _inventory.EquipWeapon(weapon);
        _draggedIndex = -1;
    }

    public void DropOnRemove()
    {
        if (_inventory == null || _contents == null) return;
        if (_draggedIndex < 0 || _draggedIndex >= _contents.Length) return;
        Weapon weapon = _contents[_draggedIndex].weapon;
        if (weapon != null) _inventory.RemoveWeapon(weapon);
        _draggedIndex = -1;
    }

    public void EndDrag()
    {
        if (_dragGhost != null) Destroy(_dragGhost);
        _dragGhost = null;
        _draggedIndex = -1;
        ApplyContents();
        RefreshHoverInfo();
    }

    private void CollectSlots()
    {
        _slots.Clear();
        if (panel == null) return;
        for (int row = 1; row <= 4; row++)
        {
            Transform rowRoot = panel.transform.Find("Slot_" + row);
            if (rowRoot == null) continue;
            for (int i = 0; i < rowRoot.childCount; i++)
            {
                InventorySlotUI slot = rowRoot.GetChild(i).GetComponent<InventorySlotUI>();
                if (slot == null) continue;
                slot.Initialize(this, _slots.Count);
                _slots.Add(slot);
            }
        }
    }

    private void Refresh()
    {
        if (_inventory == null || _slots.Count == 0) return;
        SyncContents();
        ApplyContents();
        RefreshHoverInfo();
    }

    private void SyncContents()
    {
        if (_contents == null || _contents.Length != _slots.Count)
        {
            _contents = new SlotContent[_slots.Count];
        }

        for (int i = 0; i < _contents.Length; i++)
        {
            Weapon weapon = _contents[i].weapon;
            if (weapon != null && !_inventory.HasWeapon(weapon)) _contents[i] = default;
        }

        IReadOnlyList<Weapon> weapons = _inventory.Weapons;
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon weapon = weapons[i];
            if (weapon == null || ContainsWeapon(weapon)) continue;
            int slot = FindEmptySlot();
            if (slot < 0) break;
            _contents[slot] = new SlotContent { icon = weapon.icon, weapon = weapon };
        }

        for (int i = 0; i < _contents.Length; i++)
        {
            Weapon weapon = _contents[i].weapon;
            if (weapon == null) continue;
            _contents[i].icon = weapon.icon;
            _contents[i].quantity = _inventory.GetWeaponQuantity(weapon);
        }
    }

    private bool ContainsWeapon(Weapon weapon)
    {
        for (int i = 0; i < _contents.Length; i++)
            if (_contents[i].weapon == weapon) return true;
        return false;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < _contents.Length; i++)
            if (_contents[i].weapon == null) return i;
        return -1;
    }

    private void ApplyContents()
    {
        if (_contents == null) return;
        for (int i = 0; i < _slots.Count && i < _contents.Length; i++)
        {
            _slots[i].SetItem(_contents[i].icon, _contents[i].quantity);
        }
    }

    private void RefreshHoverInfo()
    {
        Weapon weapon = _contents != null && _hoveredIndex >= 0 && _hoveredIndex < _contents.Length
            ? _contents[_hoveredIndex].weapon
            : null;
        if (infoPanel == null) return;
        if (weapon == null)
        {
            infoPanel.Hide();
            return;
        }
        infoPanel.Show(weapon);
    }
}
