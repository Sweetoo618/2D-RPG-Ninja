using TMPro;
using UnityEngine;

public sealed class QuestUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Canvas canvas;
    private TMP_Text _text;

    private void Awake()
    {
        if (questManager == null) questManager = FindObjectOfType<QuestManager>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        GameObject root = new GameObject("QuestTracker", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        RectTransform rect = (RectTransform)root.transform;
        rect.anchorMin = new Vector2(0.02f, 0.82f);
        rect.anchorMax = new Vector2(0.36f, 0.96f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        GameObject label = new GameObject("QuestText", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);
        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        _text = label.GetComponent<TMP_Text>();
        _text.fontSize = 18f;
        _text.color = Color.white;
        _text.raycastTarget = false;
        if (questManager != null) questManager.Changed += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (questManager != null) questManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (_text == null) return;
        _text.text = questManager == null ? string.Empty : questManager.CurrentQuestText;
        _text.color = questManager != null && questManager.IsComplete ? new Color(0.35f, 1f, 0.55f) : Color.white;
    }
}
