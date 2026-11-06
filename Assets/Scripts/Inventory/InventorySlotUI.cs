using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Controls one inventory slot's visual state, item display and drag source.</summary>
public sealed class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image defaultImage;
    [SerializeField] private Image hoverImage;
    [SerializeField] private Image checkImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text countText;

    private InventoryUI _owner;
    private int _index;
    private Sprite _icon;
    private int _quantity;
    private bool _isHovered;
    private bool _isPressed;
    private bool _isSelected;

    private void Awake()
    {
        ApplyVisualState();
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

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        ApplyVisualState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        ApplyVisualState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        _isPressed = false;
        ApplyVisualState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData != null && eventData.button != PointerEventData.InputButton.Left) return;
        _isPressed = true;
        ApplyVisualState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        ApplyVisualState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button != PointerEventData.InputButton.Left) return;
        _owner?.SelectSlot(_index);
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

    private void ApplyVisualState()
    {
        bool isChecked = _isSelected || _isPressed;
        if (defaultImage != null) defaultImage.gameObject.SetActive(!_isHovered && !isChecked);
        if (hoverImage != null) hoverImage.gameObject.SetActive(_isHovered && !isChecked);
        if (checkImage != null) checkImage.gameObject.SetActive(isChecked);
    }
}
