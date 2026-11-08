using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话显示层：引用场景中手工搭建的 DialoguePanel，不再用代码生成。
/// </summary>
public sealed class DialogueUI : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("对话面板根节点（未打开时隐藏）")]
    [SerializeField] private GameObject panel;
    [Tooltip("说话人文本")]
    [SerializeField] private TMP_Text speaker;
    [Tooltip("对话正文")]
    [SerializeField] private TMP_Text body;
    [Tooltip("头像")]
    [SerializeField] private Image portrait;

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        DialogueManager manager = DialogueManager.Instance;
        if (manager == null || !DialogueManager.IsOpen) return;
        // 打开对话的那一帧不推进，避免开启用的 E 同时被当成翻页
        if (Time.frameCount == manager.OpenedFrame) return;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            manager.Advance();
        }
    }

    public void ShowLine(NPCDialogue dialogue, int lineIndex)
    {
        if (panel == null || dialogue == null) return;

        NPCDialogueLine line = dialogue.lines[lineIndex];
        if (speaker != null)
            speaker.text = string.IsNullOrWhiteSpace(line.speaker) ? dialogue.npcName : line.speaker;
        if (body != null)
            body.text = line.text;
        if (portrait != null)
        {
            portrait.sprite = dialogue.portrait;
            portrait.enabled = portrait.sprite != null;
        }
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}