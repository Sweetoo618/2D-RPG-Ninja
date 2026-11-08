using UnityEngine;

public sealed class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Food[] foods;

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

        if (foods == null) return;
        for (int i = 0; i < foods.Length; i++)
        {
            Food food = foods[i];
            if (food == null) continue;

            GameObject item = Instantiate(itemPrefab, content);
            if (item.TryGetComponent(out ShopItemUI itemUI))
                itemUI.Bind(food, _inventory, _stats);
        }
    }
}
