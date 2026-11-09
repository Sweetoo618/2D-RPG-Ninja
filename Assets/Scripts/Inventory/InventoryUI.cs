using System.Collections.Generic;
using System.Linq;
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
        public InventoryItemStack item;
    }

    private readonly List<InventorySlotUI> _slots = new List<InventorySlotUI>();
    private PlayerInventory _inventory;
    private SlotContent[] _contents;
    private Canvas _canvas;
    private GameObject _dragGhost;
    private int _draggedIndex = -1;
    private int _selectedIndex = -1;
    private bool _isOpen;

    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _inventory = FindObjectOfType<PlayerInventory>();
        _canvas = GetComponent<Canvas>();
        if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        if (panel == null) panel = transform.Find("Inventory-Panel")?.gameObject;
        CollectSlots();
        if (_inventory != null) _inventory.Changed += Refresh;
        _isOpen = panel != null && panel.activeSelf;
        Refresh();
        ClearSelection();
    }

    private void OnDestroy()
    {
        if (_inventory != null) _inventory.Changed -= Refresh;
    }

    public void Open()
    {
        if (panel == null) return;
        _isOpen = true;
        panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        _isOpen = false;
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

    public void SelectSlot(int index)
    {
        if (_contents == null || index < 0 || index >= _contents.Length) return;
        if (_contents[index].weapon == null && _contents[index].item == null)
        {
            ClearSelection();
            return;
        }
        _selectedIndex = index;
        ApplySelectionVisuals();
        RefreshSelectedInfo();
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
            if (_selectedIndex == _draggedIndex) _selectedIndex = targetIndex;
            else if (_selectedIndex == targetIndex) _selectedIndex = _draggedIndex;
        }
        _draggedIndex = -1;
        ApplyContents();
        ApplySelectionVisuals();
        RefreshSelectedInfo();
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

        SlotContent content = _contents[_draggedIndex];
        if (content.weapon != null) _inventory.RemoveWeapon(content.weapon);
        else if (content.item != null) _inventory.RemoveItemStack(content.item);

        _draggedIndex = -1;
    }

    public void DropOnUse()
    {
        if (_inventory == null || _contents == null) return;
        if (_draggedIndex < 0 || _draggedIndex >= _contents.Length) return;

        InventoryItemStack item = _contents[_draggedIndex].item;
        if (item != null) _inventory.UseItem(item);

        _draggedIndex = -1;
    }

    public void EndDrag()
    {
        if (_dragGhost != null) Destroy(_dragGhost);
        _dragGhost = null;
        _draggedIndex = -1;
        ApplyContents();
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
        RefreshSelectedInfo();
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
            InventoryItemStack item = _contents[i].item;
            if (weapon != null && !_inventory.HasWeapon(weapon)) _contents[i] = default;
            else if (weapon == null && item != null && !_inventory.Items.Contains(item)) _contents[i] = default;
        }

        IReadOnlyList<Weapon> weapons = _inventory.Weapons;
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon weapon = weapons[i];
            if (weapon == null || ContainsWeapon(weapon)) continue;
            int slot = FindEmptySlot();
            if (slot < 0) break;
            _contents[slot].weapon = weapon;
        }

        IReadOnlyList<InventoryItemStack> items = _inventory.Items;
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItemStack item = items[i];
            if (item == null || ContainsItem(item)) continue;
            int slot = FindEmptySlot();
            if (slot < 0) break;
            _contents[slot].item = item;
        }

        for (int i = 0; i < _contents.Length; i++)
        {
            if (_contents[i].weapon != null)
            {
                _contents[i].icon = _contents[i].weapon.icon;
                _contents[i].quantity = _inventory.GetWeaponQuantity(_contents[i].weapon);
            }
            else if (_contents[i].item != null)
            {
                _contents[i].icon = _contents[i].item.icon;
                _contents[i].quantity = _contents[i].item.quantity;
            }
        }
    }

    private bool ContainsWeapon(Weapon weapon)
    {
        for (int i = 0; i < _contents.Length; i++)
            if (_contents[i].weapon == weapon) return true;
        return false;
    }

    private bool ContainsItem(InventoryItemStack item)
    {
        for (int i = 0; i < _contents.Length; i++)
            if (_contents[i].item == item) return true;
        return false;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < _contents.Length; i++)
            if (_contents[i].weapon == null && _contents[i].item == null) return i;
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

    private void RefreshSelectedInfo()
    {
        if (_contents == null || _selectedIndex < 0 || _selectedIndex >= _contents.Length)
        {
            ClearSelection();
            return;
        }

        Weapon weapon = _contents[_selectedIndex].weapon;
        if (weapon != null)
        {
            if (infoPanel != null) infoPanel.Show(weapon);
            return;
        }

        InventoryItemStack item = _contents[_selectedIndex].item;
        if (item != null)
        {
            if (infoPanel != null) infoPanel.Show(item);
            return;
        }

        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectedIndex = -1;
        ApplySelectionVisuals();
        if (infoPanel != null) infoPanel.Hide();
    }

    private void ApplySelectionVisuals()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetSelected(i == _selectedIndex);
        }
    }
}
