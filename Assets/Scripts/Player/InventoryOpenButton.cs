using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Scene-mounted button entry point for the inventory panel.</summary>
public sealed class InventoryOpenButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void OpenInventory()
    {
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryUI?.Open();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenInventory();
    }
}
