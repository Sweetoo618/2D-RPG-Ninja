using System;
using UnityEngine;

[Serializable]
public struct NPCDialogueLine
{
    [SerializeField] private string speaker;
    [TextArea(2, 5)]
    [SerializeField] private string text;
    [SerializeField] private Sprite portrait;

    public string Speaker => speaker;
    public string Text => text;
    public Sprite Portrait => portrait;

    public NPCDialogueLine(string speaker, string text, Sprite portrait = null)
    {
        this.speaker = speaker;
        this.text = text;
        this.portrait = portrait;
    }
}

public sealed class NPCDialogue : MonoBehaviour
{
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private Sprite portrait;
    [SerializeField] private NPCDialogueLine[] lines;

    public string NPCName => npcName;
    public Sprite Portrait => portrait;
    public NPCDialogueLine[] Lines => lines;

    public void Configure(string name, Sprite defaultPortrait, NPCDialogueLine[] dialogueLines)
    {
        npcName = name;
        portrait = defaultPortrait;
        lines = dialogueLines ?? Array.Empty<NPCDialogueLine>();
    }

    public bool HasDialogue => lines != null && lines.Length > 0;
}
