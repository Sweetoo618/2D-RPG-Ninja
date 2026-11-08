using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 已接任务面板：顶部显示当前进行中的任务，下方保留所有已完成的任务。
/// </summary>
public sealed class QuestPanelUI : MonoBehaviour
{
    [Tooltip("任务行的父节点（Content）")]
    [SerializeField] private Transform content;

    [Tooltip("任务行预制体（Quest）")]
    [SerializeField] private GameObject rowPrefab;

    private QuestManager _manager;

    private void OnEnable()
    {
        _manager = FindObjectOfType<QuestManager>();
        if (_manager != null) _manager.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (_manager != null) _manager.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (content == null || rowPrefab == null) return;

        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        if (_manager == null) return;

        if (_manager.ActiveQuest != null)
            SpawnRow(_manager.ActiveQuest);

        IReadOnlyList<QuestRecord> completed = _manager.CompletedQuests;
        for (int i = 0; i < completed.Count; i++)
            SpawnRow(completed[i]);
    }

    private void SpawnRow(QuestRecord record)
    {
        GameObject row = Instantiate(rowPrefab, content);
        if (row.TryGetComponent(out QuestRowUI rowUI)) rowUI.Bind(record);
    }
}