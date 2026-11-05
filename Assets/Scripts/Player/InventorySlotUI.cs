using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Controls one inventory slot's hover, item display and drag source.</summary>
public sealed class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image defaultImage;
    [SerializeField] private Image checkImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text countText;

    private InventoryUI _owner;
    private int _index;
    private Sprite _icon;
    private int _quantity;
    private bool _isHovered;

    private void Awake()
    {
        ApplyHoverState();
        ApplyItem();
    }

    public void Initialize(InventoryUI owner, int index)
    {
        _owner = owner;
        _index = index;
    }

    public void SetItem(Sprite icon, int quantity)
    {
        _icon = icon;
        _quantity = quantity;
        ApplyItem();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        ApplyHoverState();
        _owner?.HoverSlot(_index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        ApplyHoverState();
        _owner?.ClearHover(_index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_owner == null || _icon == null) return;
        if (itemImage != null) itemImage.enabled = false;
        _owner.BeginDrag(_index, _icon, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _owner?.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _owner?.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        _owner?.DropOn(_index);
    }

    private void ApplyItem()
    {
        bool hasItem = _icon != null;
        if (itemImage != null)
        {
            itemImage.sprite = _icon;
            itemImage.enabled = hasItem;
        }
        if (countText != null)
        {
            countText.text = hasItem ? _quantity.ToString() : string.Empty;
        }
    }

    private void ApplyHoverState()
    {
        if (defaultImage != null) defaultImage.gameObject.SetActive(!_isHovered);
        if (checkImage != null) checkImage.gameObject.SetActive(_isHovered);
    }
}
