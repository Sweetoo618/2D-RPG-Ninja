using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Scene-mounted button that toggles the inventory panel.</summary>
public sealed class InventoryOpenButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null) return;
        if (inventoryUI.IsOpen) inventoryUI.Close();
        else inventoryUI.Open();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleInventory();
    }
}
