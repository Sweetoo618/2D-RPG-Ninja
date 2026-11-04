using UnityEngine;

public sealed class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<DialogueManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    public static bool IsOpen => Instance != null && Instance._activeDialogue != null;

    [SerializeField] private DialogueUI dialogueUI;
    private NPCDialogue _activeDialogue;
    private int _lineIndex;

    public NPCDialogue ActiveDialogue => _activeDialogue;
    public int CurrentLineIndex => _lineIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (dialogueUI == null) dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null) dialogueUI.Hide();
    }

    private void OnEnable()
    {
        if (_instance == null) _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public void Begin(NPCDialogue dialogue)
    {
        if (dialogue == null || !dialogue.HasDialogue || dialogueUI == null) return;
        _activeDialogue = dialogue;
        _lineIndex = 0;
        dialogueUI.ShowLine(_activeDialogue, _lineIndex);
    }

    public void Advance()
    {
        if (!IsOpen) return;
        _lineIndex++;
        if (_lineIndex >= _activeDialogue.Lines.Length)
        {
            End();
            return;
        }

        dialogueUI.ShowLine(_activeDialogue, _lineIndex);
    }

    public void End()
    {
        _activeDialogue = null;
        _lineIndex = 0;
        if (dialogueUI != null) dialogueUI.Hide();
    }
}
