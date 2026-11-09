using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    [System.Serializable]
    private class SaveData
    {
        public int level, attributePoints, strength, vitality, intelligence, defense;
        public int gold;
        public float health, mana, maxHealth, maxMana, experience, nextLevelExperience;
        public string scene;
        public float positionX, positionY;
        public string activeQuestId;
        public int questProgress;
        public string[] completedQuestIds;
        public string characterName;
        public string[] weaponNames;
        public int[] weaponQuantities;
        public int equippedWeaponIndex;
        public string[] itemIds;
        public int[] itemQuantities;
    }

    private static string[] _pendingWeaponNames;
    private static int[] _pendingWeaponQuantities;
    private static int _pendingEquippedWeaponIndex;
    private static string[] _pendingItemIds;
    private static int[] _pendingItemQuantities;
    private static bool _hasPendingInventory;

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(PlayerStats stats, Transform playerTransform = null, QuestManager questManager = null, PlayerInventory inventory = null)
    {
        if (stats == null) return;
        SaveData data = new SaveData
        {
            level = stats.Level, attributePoints = stats.AttributePoints,
            strength = stats.Strength, vitality = stats.Vitality,
            intelligence = stats.Intelligence, defense = stats.Defense,
            gold = stats.Gold,
            health = stats.Health, mana = stats.Mana, experience = stats.CurrentExp,
            maxHealth = stats.MaxHealth, maxMana = stats.MaxMana,
            nextLevelExperience = stats.NextLevelExp, scene = SceneManager.GetActiveScene().name,
            positionX = playerTransform == null ? 0f : playerTransform.position.x,
            positionY = playerTransform == null ? 0f : playerTransform.position.y,
            activeQuestId = questManager == null ? string.Empty : questManager.ActiveQuestId,
            questProgress = questManager == null ? 0 : questManager.Progress,
            completedQuestIds = questManager == null ? null : questManager.CompletedQuestIds,
            characterName = CharacterSelection.Selected == null ? string.Empty : CharacterSelection.Selected.name,
            weaponNames = inventory == null ? null : inventory.GetWeaponNames(),
            weaponQuantities = inventory == null ? null : inventory.GetWeaponQuantities(),
            equippedWeaponIndex = inventory == null ? -1 : inventory.EquippedIndex,
            itemIds = inventory == null ? null : inventory.GetItemIds(),
            itemQuantities = inventory == null ? null : inventory.GetItemQuantities()
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public static bool Load(PlayerStats stats, out string scene, out Vector3 position, out string activeQuestId, out int questProgress, out string[] completedQuestIds, PlayerInventory inventory = null)
    {
        scene = null;
        position = Vector3.zero;
        activeQuestId = string.Empty;
        questProgress = 0;
        completedQuestIds = null;
        if (stats == null || !File.Exists(SavePath)) return false;
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        if (data == null) return false;
        stats.Level = data.level; stats.AttributePoints = data.attributePoints;
        stats.Strength = data.strength; stats.Vitality = data.vitality;
        stats.Intelligence = data.intelligence; stats.Defense = data.defense;
        stats.Gold = Mathf.Max(0, data.gold);
        if (data.maxHealth > 0f) stats.MaxHealth = data.maxHealth;
        if (data.maxMana > 0f) stats.MaxMana = data.maxMana;
        stats.Health = Mathf.Clamp(data.health, 0f, stats.MaxHealth);
        stats.Mana = Mathf.Clamp(data.mana, 0f, stats.MaxMana);
        stats.CurrentExp = data.experience; stats.NextLevelExp = data.nextLevelExperience;
        scene = data.scene;
        position = new Vector3(data.positionX, data.positionY, 0f);
        activeQuestId = data.activeQuestId;
        questProgress = data.questProgress;
        completedQuestIds = data.completedQuestIds;
        CharacterSelection.Selected = ResolveCharacter(data.characterName);
        _pendingWeaponNames = data.weaponNames;
        _pendingWeaponQuantities = data.weaponQuantities;
        _pendingEquippedWeaponIndex = data.equippedWeaponIndex;
        _pendingItemIds = data.itemIds;
        _pendingItemQuantities = data.itemQuantities;
        _hasPendingInventory = true;
        ApplyPendingInventory(inventory);
        return true;
    }

    public static void ApplyPendingInventory(PlayerInventory inventory)
    {
        if (!_hasPendingInventory || inventory == null) return;
        inventory.RestoreByNames(_pendingWeaponNames, _pendingWeaponQuantities, _pendingEquippedWeaponIndex);
        inventory.RestoreItems(_pendingItemIds, _pendingItemQuantities);
        _pendingWeaponNames = null;
        _pendingWeaponQuantities = null;
        _pendingItemIds = null;
        _pendingItemQuantities = null;
        _pendingEquippedWeaponIndex = -1;
        _hasPendingInventory = false;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }

    private static CharacterDefinition ResolveCharacter(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        CharacterDefinition[] characters = Resources.FindObjectsOfTypeAll<CharacterDefinition>();
        for (int i = 0; i < characters.Length; i++)
            if (characters[i] != null && characters[i].name == name) return characters[i];
        return null;
    }

    public static bool HasSave => File.Exists(SavePath);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        _pendingWeaponNames = null;
        _pendingWeaponQuantities = null;
        _pendingEquippedWeaponIndex = -1;
        _pendingItemIds = null;
        _pendingItemQuantities = null;
        _hasPendingInventory = false;
    }
}