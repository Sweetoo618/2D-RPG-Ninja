using TMPro;
using UnityEngine;

public sealed class NPCInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private NPCDialogue dialogue;
    [SerializeField] private float interactionRadius = 1.35f;
    [SerializeField] private Transform player;

    private bool _playerInRange;
    private TMP_Text _prompt;

    public NPCDialogue Dialogue => dialogue;
    public bool PlayerInRange => _playerInRange;

    private void Awake()
    {
        if (dialogue == null) dialogue = GetComponent<NPCDialogue>();
        CreatePrompt();
    }

    private void Start()
    {
        if (player == null)
        {
            Player playerComponent = FindObjectOfType<Player>();
            player = playerComponent != null ? playerComponent.transform : null;
        }
    }

    private void Update()
    {
        if (player == null || dialogue == null || !dialogue.HasDialogue)
        {
            SetPromptVisible(false);
            return;
        }

        _playerInRange = Vector2.Distance(transform.position, player.position) <= interactionRadius;
        SetPromptVisible(_playerInRange && !DialogueManager.IsOpen);

        if (_playerInRange && !DialogueManager.IsOpen && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.Begin(dialogue);
        }
    }

    private void CreatePrompt()
    {
        GameObject promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 1.25f, 0f);
        _prompt = promptObject.AddComponent<TextMeshPro>();
        _prompt.alignment = TextAlignmentOptions.Center;
        _prompt.fontSize = 2.2f;
        _prompt.color = Color.white;
        _prompt.text = "E Talk";
        SetPromptVisible(false);
    }

    private void SetPromptVisible(bool visible)
    {
        if (_prompt != null) _prompt.gameObject.SetActive(visible);
    }
}
