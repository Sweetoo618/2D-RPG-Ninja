using UnityEngine;

[CreateAssetMenu(menuName = "Game/Quest", fileName = "Quest_")]
public sealed class QuestDefinition : ScriptableObject
{
    public string questId;
    public string title;
    [TextArea] public string description;
    public int targetKills = 1;
    public int experienceReward = 100;
}
