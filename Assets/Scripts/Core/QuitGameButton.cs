using UnityEngine;
using UnityEngine.UI;

/// <summary>点击退出游戏，编辑器内停止播放。</summary>
[RequireComponent(typeof(Button))]
public sealed class QuitGameButton : MonoBehaviour
{
    private void Awake() => GetComponent<Button>().onClick.AddListener(Quit);

    private void OnDestroy() => GetComponent<Button>().onClick.RemoveListener(Quit);

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
