using TMPro;
using UnityEngine;

/// <summary>
/// Bridges NPC interaction and the quest state machine. A quest can be authored
/// in a QuestDefinition asset, while the fallback keeps the sample scene playable
/// when no asset has been assigned yet.
/// </summary>
public sealed class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestDefinition quest;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private string questTitle = "Clear the Enemy Area";
    [SerializeField] private int targetKills = 3;
    [SerializeField] private int experienceReward = 150;

    private Player _player;
    private QuestManager _manager;
    private TMP_Text _prompt;

    private void Awake()
    {
        // The NPC's dialogue prompt remains the single interaction affordance.
        // Quest input is available while the player is in the same interaction range.
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _manager = FindObjectOfType<QuestManager>();
        if (quest == null)
        {
            quest = ScriptableObject.CreateInstance<QuestDefinition>();
            quest.questId = name + "_Quest";
            quest.title = questTitle;
            quest.description = "Defeat enemies in the enemy area, then return for your reward.";
            quest.targetKills = targetKills;
            quest.experienceReward = experienceReward;
        }
    }

    private void Update()
    {
        if (_player == null) _player = FindObjectOfType<Player>();
        if (_manager == null) _manager = FindObjectOfType<QuestManager>();
        bool inRange = _player != null && Vector2.Distance(transform.position, _player.transform.position) <= interactionRadius;
        bool canAct = inRange && !DialogueManager.IsOpen && _manager != null;
        if (!canAct) return;
        if (Input.GetKeyDown(KeyCode.Q)) _manager.TryAccept(quest);
        if (Input.GetKeyDown(KeyCode.E) && _manager.IsComplete) _manager.ClaimReward();
    }
}
