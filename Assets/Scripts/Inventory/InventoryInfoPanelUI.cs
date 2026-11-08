using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows the selected inventory item's (weapon / consumable) icon, name and description.</summary>
public sealed class InventoryInfoPanelUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    public void Show(Weapon weapon)
    {
        if (weapon == null)
        {
            Hide();
            return;
        }
        Apply(weapon.icon, weapon.weaponName, weapon.description);
    }

    public void Show(InventoryItemStack item)
    {
        if (item == null)
        {
            Hide();
            return;
        }
        Apply(item.icon, item.displayName, item.description);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Apply(Sprite icon, string title, string description)
    {
        gameObject.SetActive(true);
        if (itemImage != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = icon != null;
        }
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
    }
}