using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>玩家死亡面板：显示后仅能点击退出按钮返回开始场景。</summary>
public sealed class PlayerDeathPanel : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private string startSceneName = "StartScene";

    private void Awake() => exitButton.onClick.AddListener(ExitToStartScene);

    private void OnDestroy() => exitButton.onClick.RemoveListener(ExitToStartScene);

    public void Show() => gameObject.SetActive(true);

    private void ExitToStartScene() => SceneManager.LoadScene(startSceneName);
}
