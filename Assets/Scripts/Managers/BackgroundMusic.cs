using UnityEngine;

/// <summary>跨场景持续播放的背景音乐：挂在带 AudioSource 的物体上，切场景时不被销毁。</summary>
public sealed class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}