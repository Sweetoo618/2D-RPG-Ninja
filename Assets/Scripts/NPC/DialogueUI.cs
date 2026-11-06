using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class DialogueUI : MonoBehaviour
{
    [Header("Existing art")]
    [SerializeField] private Sprite dialogueBoxSprite;
    [SerializeField] private Sprite portraitFrameSprite;
    [SerializeField] private Sprite arrowSprite;

    private GameObject _root;
    private Image _box;
    private Image _portrait;
    private Image _portraitFrame;
    private Image _arrow;
    private TMP_Text _speaker;
    private TMP_Text _body;
    private TMP_Text _hint;

    private void Awake()
    {
        Build();
        Hide();
    }

    private void Update()
    {
        if (DialogueManager.IsOpen &&
            (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            DialogueManager.Instance.Advance();
        }
    }

    public void ShowLine(NPCDialogue dialogue, int lineIndex)
    {
        if (_root == null) Build();
        NPCDialogueLine line = dialogue.Lines[lineIndex];
        _speaker.text = string.IsNullOrWhiteSpace(line.Speaker) ? dialogue.NPCName : line.Speaker;
        _body.text = line.Text;
        _portrait.sprite = line.Portrait != null ? line.Portrait : dialogue.Portrait;
        _portrait.enabled = _portrait.sprite != null;
        _root.SetActive(true);
    }

    public void Hide()
    {
        if (_root != null) _root.SetActive(false);
    }

    private void Build()
    {
        if (_root != null) return;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        _root = new GameObject("DialoguePanel", typeof(RectTransform), typeof(Image));
        _root.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = (RectTransform)_root.transform;
        panelRect.anchorMin = new Vector2(0.08f, 0.06f);
        panelRect.anchorMax = new Vector2(0.92f, 0.32f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        _box = _root.GetComponent<Image>();
        _box.sprite = dialogueBoxSprite;
        _box.type = dialogueBoxSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        _box.color = Color.white;

        _portraitFrame = CreateImage("PortraitFrame", _root.transform, portraitFrameSprite,
            new Vector2(0.03f, 0.12f), new Vector2(0.22f, 0.88f));
        _portrait = CreateImage("Portrait", _portraitFrame.transform, null,
            new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));

        _speaker = UIFactory.CreateText("Speaker", _root.transform, new Vector2(0.26f, 0.68f), new Vector2(0.96f, 0.9f), 28f);
        _speaker.color = new Color(0.12f, 0.08f, 0.05f);
        _body = UIFactory.CreateText("Body", _root.transform, new Vector2(0.26f, 0.22f), new Vector2(0.94f, 0.67f), 24f);
        _body.color = new Color(0.08f, 0.08f, 0.08f);
        _body.enableWordWrapping = true;
        _hint = UIFactory.CreateText("Hint", _root.transform, new Vector2(0.7f, 0.04f), new Vector2(0.96f, 0.2f), 16f);
        _hint.alignment = TextAlignmentOptions.Right;
        _hint.color = new Color(0.15f, 0.12f, 0.08f);
        _hint.text = "E / Space  Next";
        _arrow = CreateImage("Arrow", _root.transform, arrowSprite, new Vector2(0.94f, 0.08f), new Vector2(0.985f, 0.2f));
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Vector2 min, Vector2 max)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)imageObject.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.enabled = sprite != null;
        return image;
    }
}
