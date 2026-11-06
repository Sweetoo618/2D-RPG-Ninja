using UnityEngine;

/// <summary>
/// Central catalog of inventory item definitions. Resolves an item id to a
/// concrete stack, decoupling item data from the inventory model and the shop.
/// </summary>
public static class ItemDatabase
{
    public static InventoryItemStack Create(string itemId, int amount = 1)
    {
        switch (itemId)
        {
            case "LifePotion": return new InventoryItemStack(itemId, "Life Potion", ItemIconResolver.Resolve("LifePotion"), amount, 18, 8f, 0f);
            case "Beaf": return new InventoryItemStack(itemId, "Beaf", ItemIconResolver.Resolve("Beaf"), amount, 12, 5f, 0f);
            case "Shrimp": return new InventoryItemStack(itemId, "Shrimp", ItemIconResolver.Resolve("Shrimp"), amount, 10, 4f, 0f);
            case "TeaLeaf": return new InventoryItemStack(itemId, "Tea Leaf", ItemIconResolver.Resolve("TeaLeaf"), amount, 5, 0f, 8f);
            case "Fish": return new InventoryItemStack(itemId, "Fish", ItemIconResolver.Resolve("Fish"), amount, 10, 6f, 0f);
            case "Honey": return new InventoryItemStack(itemId, "Honey", ItemIconResolver.Resolve("Honey"), amount, 8, 3f, 3f);
            default: return null;
        }
    }
}