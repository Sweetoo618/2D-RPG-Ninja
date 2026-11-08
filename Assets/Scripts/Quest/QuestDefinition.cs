using System;
using UnityEngine;

[Serializable]
public struct QuestItemReward
{
    [Tooltip("奖励道具（Food / Potion）")]
    public Consumable item;

    [Tooltip("数量")]
    public int amount;
}

[CreateAssetMenu(menuName = "Game/Quest", fileName = "Quest_")]
public sealed class QuestDefinition : ScriptableObject
{
    [Header("Info")]
    [Tooltip("任务唯一标识")]
    public string questId;

    [Tooltip("任务标题")]
    public string title;

    [Tooltip("任务图片")]
    public Sprite image;

    [Header("Requirement")]
    [Tooltip("任务要求描述")]
    [TextArea]
    public string description;

    [Tooltip("需要击杀的敌人数量")]
    public int targetKills = 1;

    [Header("Reward Text")]
    [Tooltip("奖励描述（展示用）")]
    [TextArea]
    public string rewardDescription;

    [Header("Reward Values")]
    [Tooltip("经验值")]
    public int experienceReward = 100;

    [Tooltip("立即回复的生命值")]
    public int healthReward;

    [Tooltip("立即回复的法力值")]
    public int manaReward;

    [Tooltip("金币")]
    public int goldReward;

    [Tooltip("属性点")]
    public int attributePointReward;

    [Tooltip("道具奖励")]
    public QuestItemReward[] itemRewards;

    [Tooltip("武器奖励")]
    public Weapon[] weaponRewards;
}