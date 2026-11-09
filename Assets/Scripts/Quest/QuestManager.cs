using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class QuestManager : MonoBehaviour
{
    [Tooltip("读档恢复时用于还原进行中任务的默认任务（旧存档不保存任务 id）")]
    [SerializeField] private QuestDefinition defaultQuest;

    private readonly List<QuestRecord> _records = new List<QuestRecord>();

    public event Action Changed;

    /// <summary>当前进行中（未领奖）的任务，无则为 null。</summary>
    public QuestRecord ActiveQuest { get; private set; }

    public int Progress => ActiveQuest == null ? 0 : ActiveQuest.progress;

    public bool HasActiveQuest => ActiveQuest != null;

    public bool IsComplete => ActiveQuest != null && ActiveQuest.IsComplete;

    public string ActiveQuestId => ActiveQuest != null && ActiveQuest.quest != null ? ActiveQuest.quest.name : string.Empty;

    public string[] CompletedQuestIds
    {
        get
        {
            List<string> ids = new List<string>();
            for (int i = 0; i < _records.Count; i++)
                if (_records[i].claimed && _records[i].quest != null)
                    ids.Add(_records[i].quest.name);
            return ids.ToArray();
        }
    }

    /// <summary>已完成（已领奖）的任务，按接取顺序。</summary>
    public IReadOnlyList<QuestRecord> CompletedQuests
    {
        get
        {
            List<QuestRecord> list = new List<QuestRecord>();
            for (int i = 0; i < _records.Count; i++)
                if (_records[i].claimed) list.Add(_records[i]);
            return list;
        }
    }

    public bool TryAccept(QuestDefinition definition)
    {
        if (definition == null || HasActiveQuest) return false;

        QuestRecord record = new QuestRecord();
        record.quest = definition;
        record.progress = 0;
        record.claimed = false;

        ActiveQuest = record;
        _records.Add(record);
        Changed?.Invoke();
        return true;
    }

    public void Abandon()
    {
        if (ActiveQuest == null) return;

        _records.Remove(ActiveQuest);
        ActiveQuest = null;
        Changed?.Invoke();
    }

    public bool ClaimReward()
    {
        if (ActiveQuest == null || !ActiveQuest.IsComplete || ActiveQuest.claimed) return false;

        QuestDefinition quest = ActiveQuest.quest;

        Player player = FindObjectOfType<Player>();
        if (player != null && player.Stats != null) ApplyStatRewards(quest, player.Stats);

        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null) ApplyInventoryRewards(quest, inventory);

        FindObjectOfType<PlayerExp>()?.AddExp(quest.experienceReward);

        ActiveQuest.claimed = true;
        ActiveQuest = null;
        Changed?.Invoke();
        return true;
    }

    public void RegisterKill()
    {
        if (ActiveQuest == null || ActiveQuest.IsComplete) return;
        ActiveQuest.progress++;
        if (ActiveQuest.IsComplete) SfxManager.Instance?.PlayQuestComplete();
        Changed?.Invoke();
    }

    public void RestoreState(string activeQuestId, int progress, string[] completedQuestIds)
    {
        _records.Clear();
        ActiveQuest = null;

        QuestDefinition activeDef = ResolveQuest(activeQuestId);
        if (activeDef != null)
        {
            QuestRecord record = new QuestRecord();
            record.quest = activeDef;
            record.progress = Mathf.Clamp(progress, 0, activeDef.targetKills);
            record.claimed = false;
            _records.Add(record);
            ActiveQuest = record;
        }

        if (completedQuestIds != null)
        {
            for (int i = 0; i < completedQuestIds.Length; i++)
            {
                QuestDefinition def = ResolveQuest(completedQuestIds[i]);
                if (def == null) continue;
                QuestRecord record = new QuestRecord();
                record.quest = def;
                record.progress = def.targetKills;
                record.claimed = true;
                _records.Add(record);
            }
        }

        Changed?.Invoke();
    }

    private QuestDefinition ResolveQuest(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (defaultQuest != null && defaultQuest.name == id) return defaultQuest;
        QuestDefinition[] quests = Resources.FindObjectsOfTypeAll<QuestDefinition>();
        for (int i = 0; i < quests.Length; i++)
            if (quests[i] != null && quests[i].name == id) return quests[i];
        return null;
    }

    private void ApplyStatRewards(QuestDefinition quest, PlayerStats stats)
    {
        stats.RestoreHealth(quest.healthReward);
        stats.RestoreMana(quest.manaReward);
        if (quest.goldReward > 0)
        {
            stats.AddGold(quest.goldReward);
            SfxManager.Instance?.PlayReward();
        }
        stats.AttributePoints += quest.attributePointReward;
    }

    private void ApplyInventoryRewards(QuestDefinition quest, PlayerInventory inventory)
    {
        if (quest.itemRewards != null)
        {
            for (int i = 0; i < quest.itemRewards.Length; i++)
            {
                QuestItemReward reward = quest.itemRewards[i];
                inventory.AddConsumable(reward.item, reward.amount);
            }
        }

        if (quest.weaponRewards != null)
        {
            for (int i = 0; i < quest.weaponRewards.Length; i++)
                inventory.AddWeapon(quest.weaponRewards[i]);
        }
    }
}