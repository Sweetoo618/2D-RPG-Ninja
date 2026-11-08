using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务行（已接取）：显示任务进度，No_Button 放弃任务，Complete_Button 领取奖励。
/// 进行中任务显示当前进度；已完成任务保留在面板中、Complete_Button 变暗。
/// </summary>
public sealed class QuestRowUI : QuestRowBase
{
    [Header("Progress & Buttons")]
    [Tooltip("进度文本（Enemy Kill Number: 3/10）")]
    [SerializeField] private TMP_Text currentContent;

    [Tooltip("放弃按钮（未达标时可用）")]
    [SerializeField] private Button noButton;

    [Tooltip("领奖按钮（达标后激活，领奖后变暗）")]
    [SerializeField] private Button completeButton;

    private QuestRecord _record;

    private void Awake()
    {
        if (noButton != null) noButton.onClick.AddListener(Abandon);
        if (completeButton != null) completeButton.onClick.AddListener(Complete);
    }

    private void OnDestroy()
    {
        if (noButton != null) noButton.onClick.RemoveListener(Abandon);
        if (completeButton != null) completeButton.onClick.RemoveListener(Complete);
    }

    public void Bind(QuestRecord record)
    {
        _record = record;
        BindInfo(record == null ? null : record.quest);
        RefreshProgress();
        RefreshButtons();
    }

    private void RefreshProgress()
    {
        if (_record == null || currentContent == null) return;
        currentContent.text = $"Enemy Kill Number: {_record.progress}/{_record.quest.targetKills}";
    }

    private void RefreshButtons()
    {
        if (_record == null) return;

        bool complete = _record.IsComplete;
        bool claimed = _record.claimed;

        if (noButton != null) noButton.gameObject.SetActive(!complete && !claimed);
        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(complete || claimed);
            completeButton.interactable = !claimed;
        }
    }

    private void Abandon()
    {
        QuestManager manager = FindObjectOfType<QuestManager>();
        if (manager != null) manager.Abandon();
    }

    private void Complete()
    {
        QuestManager manager = FindObjectOfType<QuestManager>();
        if (manager != null) manager.ClaimReward();
    }
}