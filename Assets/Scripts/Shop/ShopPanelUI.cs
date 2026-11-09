using UnityEngine;

public sealed class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Food[] foods;
    [SerializeField] private Potion[] potions;
    [SerializeField] private Weapon[] weapons;

    private PlayerInventory _inventory;
    private PlayerStats _stats;

    private void Awake()
    {
        if (content == null) content = transform.Find("Loot-Data/Viewport/Content");
        if (_inventory == null) _inventory = FindObjectOfType<PlayerInventory>();
        Player player = FindObjectOfType<Player>();
        if (player != null) _stats = player.Stats;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (content == null || itemPrefab == null) return;

        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        SpawnConsumables(foods);
        SpawnConsumables(potions);
        SpawnWeapons(weapons);
    }

    private void SpawnConsumables(Consumable[] items)
    {
        if (items == null) return;
        for (int i = 0; i < items.Length; i++)
        {
            Consumable consumable = items[i];
            if (consumable == null) continue;
            ShopItemUI row = CreateRow();
            if (row != null) row.Bind(consumable, _inventory, _stats);
        }
    }

    private void SpawnWeapons(Weapon[] items)
    {
        if (items == null) return;
        for (int i = 0; i < items.Length; i++)
        {
            Weapon weapon = items[i];
            if (weapon == null) continue;
            ShopItemUI row = CreateRow();
            if (row != null) row.Bind(weapon, _inventory, _stats);
        }
    }

    private ShopItemUI CreateRow()
    {
        GameObject row = Instantiate(itemPrefab, content);
        return row != null && row.TryGetComponent(out ShopItemUI itemUI) ? itemUI : null;
    }
}