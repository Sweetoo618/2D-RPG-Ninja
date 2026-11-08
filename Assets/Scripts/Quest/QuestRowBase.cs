using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务行基类：绑定任务的图片、标题、描述与奖励描述。
/// </summary>
public abstract class QuestRowBase : MonoBehaviour
{
    [Header("Bindings")]
    [Tooltip("任务图片")]
    [SerializeField] protected Image image;

    [Tooltip("任务标题")]
    [SerializeField] protected TMP_Text title;

    [Tooltip("任务要求描述")]
    [SerializeField] protected TMP_Text description;

    [Tooltip("奖励描述")]
    [SerializeField] protected TMP_Text rewardContent;

    protected QuestDefinition Quest;

    protected void BindInfo(QuestDefinition quest)
    {
        Quest = quest;
        if (quest == null) return;

        if (image != null)
        {
            image.sprite = quest.image;
            image.enabled = image.sprite != null;
        }

        if (title != null) title.text = quest.title;
        if (description != null) description.text = quest.description;
        if (rewardContent != null) rewardContent.text = quest.rewardDescription;
    }
}