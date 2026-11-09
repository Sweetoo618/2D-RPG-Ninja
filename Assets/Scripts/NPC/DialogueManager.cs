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
    private NPCDialogueLine[] _activeLines;
    private int _lineIndex;

    public NPCDialogue ActiveDialogue => _activeDialogue;
    public int CurrentLineIndex => _lineIndex;
    public int OpenedFrame { get; private set; } = -1;

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
        _activeLines = dialogue.GetActiveLines();
        _lineIndex = 0;
        OpenedFrame = Time.frameCount;
        ShowLine();
    }

    public void Advance()
    {
        if (!IsOpen) return;

        _lineIndex++;
        if (_lineIndex >= _activeLines.Length)
        {
            NPCDialogue finished = _activeDialogue;
            End();
            finished.MarkTalked();
            if (finished.questPanel != null) finished.questPanel.SetActive(true);
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        dialogueUI.ShowLine(_activeDialogue, _activeLines[_lineIndex]);
    }

    public void End()
    {
        _activeDialogue = null;
        _lineIndex = 0;
        OpenedFrame = -1;
        if (dialogueUI != null) dialogueUI.Hide();
    }
}
