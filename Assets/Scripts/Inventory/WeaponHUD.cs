using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Scene-mounted single weapon display. The component never creates UI objects;
/// it only mirrors the weapon currently equipped by PlayerInventory.
/// </summary>
public sealed class WeaponHUD : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image backgroundImage;
    [FormerlySerializedAs("weaponIndexText")]
    [SerializeField] private Text weaponNumText;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private PlayerInventory inventory;

    private void Awake()
    {
        if (weaponImage == null)
        {
            Transform icon = transform.Find("WeaponIcon");
            weaponImage = icon == null ? GetComponent<Image>() : icon.GetComponent<Image>();
        }
        if (backgroundImage == null) backgroundImage = GetComponent<Image>();
        if (backgroundSprite == null) backgroundSprite = ItemIconResolver.Resolve("Inventory");
        if (backgroundImage != null && backgroundImage != weaponImage)
        {
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.color = Color.white;
            backgroundImage.preserveAspect = true;
        }
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
    }

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.Changed -= Refresh;
    }

    private void Refresh()
    {
        Weapon weapon = inventory == null ? null : inventory.EquippedWeapon;
        int quantity = weapon == null || inventory == null ? 0 : inventory.GetWeaponQuantity(weapon);
        if (weaponImage != null)
        {
            weaponImage.sprite = weapon == null ? null : weapon.icon;
            weaponImage.enabled = weapon != null && weapon.icon != null;
            weaponImage.preserveAspect = true;
        }
        if (weaponNumText != null)
        {
            weaponNumText.text = weapon == null ? string.Empty : quantity.ToString();
        }
    }

    private void Subscribe()
    {
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null) inventory.Changed -= Refresh;
        if (inventory != null) inventory.Changed += Refresh;
    }
}
