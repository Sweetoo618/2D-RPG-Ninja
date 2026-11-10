using UnityEngine;

/// <summary>跨场景持续播放的背景音乐：挂在带 AudioSource 的物体上，切场景时不被销毁。</summary>
public sealed class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic _instance;

    [SerializeField] private AudioSource source;

    private float _baseVolume;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        _baseVolume = source.volume;
        ApplyVolume();
        GameAudioSettings.Changed += ApplyVolume;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameAudioSettings.Changed -= ApplyVolume;
        if (_instance == this) _instance = null;
    }

    private void ApplyVolume() => source.volume = _baseVolume * GameAudioSettings.BgmVolume;
}