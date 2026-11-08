using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC 交互：玩家靠近时在头顶显示互动 UI（NPC-Interact 预制体），离开即销毁；按 E 收起 UI。
/// 对话面板后续接入。
/// </summary>
public sealed class NPCInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("NPC 对话数据（留空则自动获取同物体上的 NPCDialogue）")]
    [SerializeField] private NPCDialogue dialogue;
    [Tooltip("靠近时显示的互动 UI 预制体")]
    [SerializeField] private GameObject interactPromptPrefab;
    [Tooltip("触发显示提示的距离")]
    [SerializeField] private float interactionRadius = 1.35f;
    [Tooltip("提示相对 NPC 头顶的偏移")]
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1.25f, 0f);
    [Tooltip("玩家（留空则运行时自动查找）")]
    [SerializeField] private Transform player;

    private Canvas _canvas;
    private RectTransform _canvasRect;
    private Camera _camera;
    private RectTransform _prompt;
    private bool _dismissed;

    private void Awake()
    {
        if (dialogue == null) dialogue = GetComponent<NPCDialogue>();
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
        if (player == null || dialogue == null || !dialogue.HasDialogue || DialogueManager.IsOpen)
        {
            DestroyPrompt();
            return;
        }

        bool inRange = Vector2.Distance(transform.position, player.position) <= interactionRadius;
        if (!inRange)
        {
            _dismissed = false;
            DestroyPrompt();
            return;
        }

        if (_dismissed) return;

        EnsurePrompt();
        if (_prompt == null) return;

        _prompt.gameObject.SetActive(true);
        FollowHead();

        if (Input.GetKeyDown(KeyCode.E))
        {
            _dismissed = true;
            DestroyPrompt();
            DialogueManager.Instance.Begin(dialogue);
        }
    }

    private void EnsurePrompt()
    {
        if (_prompt != null || interactPromptPrefab == null) return;

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null) _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null) return;

        _canvasRect = (RectTransform)_canvas.transform;
        _camera = _canvas.worldCamera != null ? _canvas.worldCamera : Camera.main;
        _prompt = Instantiate(interactPromptPrefab, _canvas.transform).GetComponent<RectTransform>();
    }

    private void DestroyPrompt()
    {
        if (_prompt == null) return;
        Destroy(_prompt.gameObject);
        _prompt = null;
    }

    // 把互动 UI 放到 NPC 头顶：世界坐标 -> 屏幕 -> 画布局部坐标。
    private void FollowHead()
    {
        if (_prompt == null || _canvasRect == null || _camera == null) return;

        Vector3 screenPoint = _camera.WorldToScreenPoint(transform.position + promptOffset);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, screenPoint, _camera, out Vector2 localPoint))
        {
            _prompt.anchoredPosition = localPoint;
        }
    }
}