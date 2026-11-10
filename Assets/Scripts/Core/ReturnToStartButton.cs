using UnityEngine;
using UnityEngine.UI;

/// <summary>点击保存当前进度并返回开始场景。</summary>
[RequireComponent(typeof(Button))]
public sealed class ReturnToStartButton : MonoBehaviour
{
    [SerializeField] private string startSceneName = "StartScene";

    private void Awake() => GetComponent<Button>().onClick.AddListener(Return);

    private void OnDestroy() => GetComponent<Button>().onClick.RemoveListener(Return);

    private void Return() => SceneTransition.Instance?.LoadScene(startSceneName);
}
