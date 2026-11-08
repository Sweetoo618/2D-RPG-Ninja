using System;
using UnityEngine;

[Serializable]
public sealed class InventoryItemStack
{
    public string itemId;
    public string displayName;
    public string description;
    public Sprite icon;
    public int quantity;
    public int maxStack = 99;
    public int price;
    public float healthRestore;
    public float manaRestore;
    public float expRestore;

    public InventoryItemStack(string id, string name, Sprite itemIcon, int amount, int itemPrice,
        float healthAmount, float manaAmount, float expAmount = 0f)
    {
        itemId = id;
        displayName = name;
        icon = itemIcon;
        quantity = Mathf.Clamp(amount, 0, maxStack);
        price = Mathf.Max(0, itemPrice);
        healthRestore = Mathf.Max(0f, healthAmount);
        manaRestore = Mathf.Max(0f, manaAmount);
        expRestore = Mathf.Max(0f, expAmount);
    }

    public InventoryItemStack Clone(int amount)
    {
        return new InventoryItemStack(itemId, displayName, icon, amount, price, healthRestore, manaRestore, expRestore)
        {
            maxStack = this.maxStack,
            description = this.description
        };
    }
}

public static class ItemIconResolver
{
    public static Sprite Resolve(string assetName)
    {
        if (string.IsNullOrWhiteSpace(assetName)) return null;

#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets(assetName + " t:Sprite", new[] { "Assets" });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null && (sprite.name.Equals(assetName, StringComparison.OrdinalIgnoreCase) ||
                                   path.IndexOf("/" + assetName + ".", StringComparison.OrdinalIgnoreCase) >= 0))
                return sprite;
        }
#endif

        Sprite[] loadedSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        for (int i = 0; i < loadedSprites.Length; i++)
            if (loadedSprites[i] != null && loadedSprites[i].name.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                return loadedSprites[i];

        return null;
    }
}
