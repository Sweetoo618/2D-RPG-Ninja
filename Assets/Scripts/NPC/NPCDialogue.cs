using System;
using UnityEngine;

[Serializable]
public struct NPCDialogueLine
{
    [Tooltip("说话人（留空则使用 NPC 名称）")]
    public string speaker;

    [TextArea(2, 5)]
    [Tooltip("对话内容")]
    public string text;
}

/// <summary>
/// NPC 对话数据：名称、头像、对话内容，全部在 Inspector 中配置。
/// </summary>
public sealed class NPCDialogue : MonoBehaviour
{
    [Header("NPC Info")]
    [Tooltip("NPC 名称（对话中默认说话人）")]
    public string npcName = "NPC";

    [Tooltip("NPC 默认头像")]
    public Sprite portrait;

    [Header("Dialogue")]
    [Tooltip("首次对话内容，按顺序播放")]
    public NPCDialogueLine[] lines;

    [Header("Repeat Dialogue")]
    [Tooltip("非首次对话内容（留空则始终复用 lines）")]
    public NPCDialogueLine[] repeatLines;

    [Header("After Dialogue")]
    [Tooltip("对话结束后弹出的面板（可留空）")]
    public GameObject questPanel;

    public bool HasDialogue => lines != null && lines.Length > 0;

    public bool HasTalked { get; private set; }

    public NPCDialogueLine[] GetActiveLines()
    {
        return HasTalked && repeatLines != null && repeatLines.Length > 0 ? repeatLines : lines;
    }

    public void MarkTalked()
    {
        HasTalked = true;
    }
}