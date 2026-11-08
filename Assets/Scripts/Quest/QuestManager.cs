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
        Changed?.Invoke();
    }

    public void RestoreState(int progress, bool accepted, bool rewardClaimed)
    {
        _records.Clear();
        ActiveQuest = null;

        if (accepted && defaultQuest != null)
        {
            QuestRecord record = new QuestRecord();
            record.quest = defaultQuest;
            record.progress = Mathf.Clamp(progress, 0, defaultQuest.targetKills);
            record.claimed = rewardClaimed;
            _records.Add(record);
            if (!rewardClaimed) ActiveQuest = record;
        }

        Changed?.Invoke();
    }

    private void ApplyStatRewards(QuestDefinition quest, PlayerStats stats)
    {
        stats.RestoreHealth(quest.healthReward);
        stats.RestoreMana(quest.manaReward);
        stats.AddGold(quest.goldReward);
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