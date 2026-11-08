using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关闭指定面板：挂在按钮上，点击后隐藏目标面板。
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class PanelCloseButton : MonoBehaviour
{
    [Tooltip("要关闭的面板（留空则使用自身根物体）")]
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        if (panel == null) panel = transform.root.gameObject;
        GetComponent<Button>().onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out Button button)) button.onClick.RemoveListener(Close);
    }

    private void Close()
    {
        if (panel != null) panel.SetActive(false);
    }
}