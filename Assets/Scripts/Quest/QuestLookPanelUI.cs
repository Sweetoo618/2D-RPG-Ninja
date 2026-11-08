using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 任务列表面板：按 Assets/SO/Quest 下的所有 QuestDefinition 生成 Quest_look 行。
/// </summary>
public sealed class QuestLookPanelUI : MonoBehaviour
{
    private const string QuestFolder = "Assets/SO/Quest";

    [Tooltip("任务行的父节点（Content）")]
    [SerializeField] private Transform content;

    [Tooltip("任务行预制体（Quest_look）")]
    [SerializeField] private GameObject rowPrefab;

    [Tooltip("要展示的任务（自动同步 Assets/SO/Quest）")]
    [SerializeField] private QuestDefinition[] quests;

    private QuestManager _manager;
    private bool _populated;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        SyncQuests();
    }

    private void SyncQuests()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestDefinition", new[] { QuestFolder });
        System.Array.Sort(guids, (a, b) => string.CompareOrdinal(
            AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)));

        quests = new QuestDefinition[guids.Length];
        for (int i = 0; i < guids.Length; i++)
            quests[i] = AssetDatabase.LoadAssetAtPath<QuestDefinition>(AssetDatabase.GUIDToAssetPath(guids[i]));
    }
#endif

    private void OnEnable()
    {
        _manager = FindObjectOfType<QuestManager>();
        if (_manager != null) _manager.Changed += RefreshInteractable;

        if (!_populated)
        {
            Populate();
            _populated = true;
        }

        RefreshInteractable();
    }

    private void OnDisable()
    {
        if (_manager != null) _manager.Changed -= RefreshInteractable;
    }

    private void RefreshInteractable()
    {
        if (content == null) return;

        bool canAccept = _manager == null || !_manager.HasActiveQuest;
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).TryGetComponent(out QuestLookRowUI row))
                row.SetInteractable(canAccept);
        }
    }

    private void Populate()
    {
        if (content == null || rowPrefab == null || quests == null) return;

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] == null) continue;

            GameObject row = Instantiate(rowPrefab, content);
            if (row.TryGetComponent(out QuestLookRowUI rowUI)) rowUI.Bind(quests[i]);
        }
    }
}