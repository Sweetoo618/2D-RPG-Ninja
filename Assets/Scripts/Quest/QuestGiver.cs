using UnityEngine;

/// <summary>
/// Lets the player claim a completed quest reward while near its giver.
/// </summary>
[DefaultExecutionOrder(100)]
public sealed class QuestGiver : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;

    private Player _player;
    private QuestManager _manager;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _manager = FindObjectOfType<QuestManager>();
    }

    private void Update()
    {
        if (_player == null) _player = FindObjectOfType<Player>();
        if (_manager == null) _manager = FindObjectOfType<QuestManager>();
        if (_manager == null || DialogueManager.IsOpen || NPCInteractable.InteractionStartedThisFrame) return;

        bool inRange = _player != null && Vector2.Distance(transform.position, _player.transform.position) <= interactionRadius;
        if (inRange && Input.GetKeyDown(KeyCode.E) && _manager.IsComplete)
            _manager.ClaimReward();
    }
}
