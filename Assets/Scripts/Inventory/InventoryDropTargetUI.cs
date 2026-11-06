using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Routes an inventory drop to the equip or remove action.</summary>
public sealed class InventoryDropTargetUI : MonoBehaviour, IDropHandler
{
    public enum DropAction
    {
        Equip,
        Remove
    }

    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private DropAction action;

    public void OnDrop(PointerEventData eventData)
    {
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null) return;
        if (action == DropAction.Equip) inventoryUI.DropOnEquip();
        else inventoryUI.DropOnRemove();
    }
}
