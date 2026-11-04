using System;
using UnityEngine;

public sealed class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestDefinition activeQuest;
    public int Progress { get; private set; }
    public bool IsAccepted { get; private set; }
    public bool IsComplete => IsAccepted && activeQuest != null && Progress >= activeQuest.targetKills;
    public bool IsRewardClaimed { get; private set; }
    public QuestDefinition ActiveQuest => activeQuest;
    public event Action Changed;

    public void StartQuest(QuestDefinition quest)
    {
        if (quest == null) return;
        activeQuest = quest;
        Progress = 0;
        IsAccepted = true;
        IsRewardClaimed = false;
        Changed?.Invoke();
    }

    public bool TryAccept(QuestDefinition quest)
    {
        if (quest == null || IsAccepted && activeQuest != quest) return false;
        if (IsAccepted) return true;
        StartQuest(quest);
        return true;
    }

    public bool ClaimReward()
    {
        if (!IsComplete || IsRewardClaimed || activeQuest == null) return false;
        FindObjectOfType<PlayerExp>()?.AddExp(activeQuest.experienceReward);
        IsRewardClaimed = true;
        Changed?.Invoke();
        return true;
    }

    public void RestoreState(int progress, bool accepted, bool rewardClaimed)
    {
        IsAccepted = accepted && activeQuest != null;
        IsRewardClaimed = rewardClaimed;
        Progress = activeQuest == null ? 0 : Mathf.Clamp(progress, 0, activeQuest.targetKills);
        Changed?.Invoke();
    }

    public void RegisterKill()
    {
        if (!IsAccepted || activeQuest == null || IsComplete) return;
        Progress++;
        Changed?.Invoke();
    }

    public void RestoreProgress(int progress)
    {
        IsAccepted = activeQuest != null && progress >= 0;
        IsRewardClaimed = false;
        Progress = activeQuest == null ? 0 : Mathf.Clamp(progress, 0, activeQuest.targetKills);
        Changed?.Invoke();
    }

    public string CurrentQuestText => activeQuest == null || !IsAccepted ? "No active quest" :
        IsRewardClaimed ? $"{activeQuest.title}: Reward claimed" : $"{activeQuest.title}: {Progress}/{activeQuest.targetKills}";
}
