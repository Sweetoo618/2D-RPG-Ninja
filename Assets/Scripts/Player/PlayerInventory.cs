using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Weapon[] startingWeapons = new Weapon[4];
    [SerializeField] private int capacity = 24;
    [SerializeField] private bool seedStarterItems = true;

    private readonly List<Weapon> _weapons = new List<Weapon>();
    private readonly List<InventoryItemStack> _items = new List<InventoryItemStack>();
    private int _equippedIndex;
    private Player _player;

    public IReadOnlyList<Weapon> Weapons => _weapons;
    public IReadOnlyList<InventoryItemStack> Items => _items;
    public int EquippedIndex => _equippedIndex;
    public int Capacity => capacity;
    public event Action Changed;

    public int ItemCount => _items.Count;

    public string[] GetWeaponNames()
    {
        string[] names = new string[_weapons.Count];
        for (int i = 0; i < _weapons.Count; i++) names[i] = _weapons[i] == null ? string.Empty : _weapons[i].name;
        return names;
    }

    public void RestoreByNames(string[] weaponNames, int equippedIndex)
    {
        if (weaponNames == null) return;
        _weapons.Clear();
        Weapon[] available = Resources.FindObjectsOfTypeAll<Weapon>();
        for (int i = 0; i < weaponNames.Length; i++)
        {
            if (string.IsNullOrEmpty(weaponNames[i])) continue;
            for (int j = 0; j < available.Length; j++)
            {
                if (available[j] != null && available[j].name == weaponNames[i])
                {
                    AddWeapon(available[j]);
                    break;
                }
            }
        }
        Equip(Mathf.Clamp(equippedIndex, 0, Mathf.Max(0, _weapons.Count - 1)));
        Changed?.Invoke();
    }

    private void Awake()
    {
        if (playerAttack == null) playerAttack = GetComponent<PlayerAttack>();
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        EnsureStarterContents();
        if (_weapons.Count > 0 && playerAttack != null)
            Equip(Mathf.Clamp(_equippedIndex, 0, _weapons.Count - 1));
    }

    private void EnsureStarterContents()
    {
        capacity = Mathf.Max(1, capacity);
        for (int i = 0; i < startingWeapons.Length; i++) AddWeapon(startingWeapons[i]);
        if (playerAttack != null)
        {
            AddWeapon(playerAttack.InitialWeapon);
            AddWeapon(playerAttack.SecondaryWeapon);
        }

        if (seedStarterItems && !_items.Exists(item => item != null && item.itemId == "LifePotion"))
            SeedStarterItems();
    }

    private void Update()
    {
        if (DialogueManager.IsOpen) return;
        for (int i = 0; i < Mathf.Min(9, _weapons.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) Equip(i);
        }
    }

    public bool EquipWeapon(Weapon weapon)
    {
        int index = _weapons.IndexOf(weapon);
        return index >= 0 && Equip(index);
    }

    public bool AddWeapon(Weapon weapon)
    {
        if (weapon == null || _weapons.Contains(weapon) || _weapons.Count >= capacity) return false;
        _weapons.Add(weapon);
        Changed?.Invoke();
        return true;
    }

    public bool AddItem(string itemId, string displayName, Sprite icon, int amount, int price,
        float healthRestore, float manaRestore)
    {
        if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;
        InventoryItemStack existing = _items.Find(item => item.itemId == itemId);
        if (existing != null)
        {
            existing.quantity = Mathf.Min(existing.maxStack, existing.quantity + amount);
            Changed?.Invoke();
            return true;
        }
        if (_items.Count >= capacity) return false;
        _items.Add(new InventoryItemStack(itemId, displayName, icon, amount, price, healthRestore, manaRestore));
        Changed?.Invoke();
        return true;
    }

    public bool AddItem(InventoryItemStack item, int amount = 1)
    {
        if (item == null) return false;
        return AddItem(item.itemId, item.displayName, item.icon, amount, item.price,
            item.healthRestore, item.manaRestore);
    }

    public bool RemoveItem(int index, int amount = 1)
    {
        if (index < 0 || index >= _items.Count || amount <= 0) return false;
        InventoryItemStack item = _items[index];
        item.quantity -= amount;
        if (item.quantity <= 0) _items.RemoveAt(index);
        Changed?.Invoke();
        return true;
    }

    public bool UseItem(int index)
    {
        if (index < 0 || index >= _items.Count || _player == null || _player.Stats == null) return false;
        InventoryItemStack item = _items[index];
        if (item.healthRestore <= 0f && item.manaRestore <= 0f) return false;
        _player.Stats.RestoreHealth(item.healthRestore);
        _player.Stats.RestoreMana(item.manaRestore);
        return RemoveItem(index);
    }

    public string[] GetItemIds()
    {
        string[] ids = new string[_items.Count];
        for (int i = 0; i < _items.Count; i++) ids[i] = _items[i].itemId;
        return ids;
    }

    public int[] GetItemQuantities()
    {
        int[] quantities = new int[_items.Count];
        for (int i = 0; i < _items.Count; i++) quantities[i] = _items[i].quantity;
        return quantities;
    }

    public void RestoreItems(string[] itemIds, int[] quantities)
    {
        if (itemIds == null) return;
        _items.Clear();
        for (int i = 0; i < itemIds.Length; i++)
        {
            int quantity = quantities != null && i < quantities.Length ? quantities[i] : 1;
            InventoryItemStack item = CreateItem(itemIds[i], quantity);
            if (item != null) _items.Add(item);
        }
        Changed?.Invoke();
    }

    private void SeedStarterItems()
    {
        InventoryItemStack potion = CreateItem("LifePotion", 3);
        AddItem(potion, potion == null ? 0 : potion.quantity);
    }

    public static InventoryItemStack CreateItem(string itemId, int amount = 1)
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

    public bool Equip(int index)
    {
        if (index < 0 || index >= _weapons.Count || playerAttack == null) return false;
        _equippedIndex = index;
        playerAttack.EquipWeapon(_weapons[index]);
        Changed?.Invoke();
        return true;
    }

}
