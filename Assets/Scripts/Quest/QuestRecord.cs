using System;
using UnityEngine;

/// <summary>
/// 玩家对某个任务的运行状态：击杀进度、是否已领取奖励。
/// </summary>
[Serializable]
public sealed class QuestRecord
{
    [Tooltip("对应的任务")]
    public QuestDefinition quest;

    [Tooltip("当前击杀进度")]
    public int progress;

    [Tooltip("是否已领取奖励（已完成后仍保留在任务面板）")]
    public bool claimed;

    public bool IsComplete => quest != null && progress >= quest.targetKills;
}