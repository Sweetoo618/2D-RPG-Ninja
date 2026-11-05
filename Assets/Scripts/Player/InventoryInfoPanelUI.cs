using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows the selected inventory weapon's icon, name and description.</summary>
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
        gameObject.SetActive(true);
        if (itemImage != null)
        {
            itemImage.sprite = weapon.icon;
            itemImage.enabled = weapon.icon != null;
        }
        if (titleText != null) titleText.text = weapon.weaponName;
        if (descriptionText != null) descriptionText.text = weapon.description;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
