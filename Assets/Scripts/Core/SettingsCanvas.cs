using UnityEngine;
using UnityEngine.UI;

/// <summary>设置面板：点击按钮开关面板，跨场景保留。</summary>
public sealed class SettingsCanvas : MonoBehaviour
{
    private static SettingsCanvas _instance;

    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingPanel;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        settingButton.onClick.AddListener(Toggle);
    }

    private void OnDestroy()
    {
        if (settingButton != null) settingButton.onClick.RemoveListener(Toggle);
        if (_instance == this) _instance = null;
    }

    private void Toggle()
    {
        if (settingPanel.activeSelf)
        {
            // 面板是按钮子物体，点击面板内容也会触发按钮，仅在按钮上点击时才收起
            if (!IsPointerOnButton()) return;
            settingPanel.SetActive(false);
            return;
        }
        settingPanel.SetActive(true);
    }

    private bool IsPointerOnButton()
    {
        RectTransform rect = settingButton.transform as RectTransform;
        return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, null);
    }
}
