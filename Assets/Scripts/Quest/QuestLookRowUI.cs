using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 可接取任务行：Yes_Button 接取任务。
/// </summary>
public sealed class QuestLookRowUI : QuestRowBase
{
    [Header("Accept")]
    [Tooltip("接取按钮")]
    [SerializeField] private Button acceptButton;

    private void Awake()
    {
        if (acceptButton != null) acceptButton.onClick.AddListener(Accept);
    }

    private void OnDestroy()
    {
        if (acceptButton != null) acceptButton.onClick.RemoveListener(Accept);
    }

    public void Bind(QuestDefinition quest)
    {
        BindInfo(quest);
    }

    public void SetInteractable(bool value)
    {
        if (acceptButton != null) acceptButton.interactable = value;
    }

    private void Accept()
    {
        if (Quest == null) return;

        QuestManager manager = FindObjectOfType<QuestManager>();
        if (manager == null || !manager.TryAccept(Quest)) return;

        Destroy(gameObject);
    }
}