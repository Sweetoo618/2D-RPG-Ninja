using UnityEngine;

/// <summary>全局一次性音效：玩家攻击、敌人受击/死亡/攻击、金币、任务、失败。</summary>
public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance;

    [Header("Refs")]
    [SerializeField] private AudioSource source;

    [Header("Clips")]
    [SerializeField] private AudioClip bow;
    [SerializeField] private AudioClip siu;
    [SerializeField] private AudioClip enemyAttack;
    [SerializeField] private AudioClip enemyDie;
    [SerializeField] private AudioClip enemyInjure;
    [SerializeField] private AudioClip reward;
    [SerializeField] private AudioClip questComplete;
    [SerializeField] private AudioClip failed;

    private float _baseVolume;

    private void Awake()
    {
        Instance = this;
        if (source != null) _baseVolume = source.volume;
        ApplyVolume();
        GameAudioSettings.Changed += ApplyVolume;
    }

    private void OnDestroy() => GameAudioSettings.Changed -= ApplyVolume;

    private void ApplyVolume()
    {
        if (source != null) source.volume = _baseVolume * GameAudioSettings.SoundVolume;
    }

    public void PlayBow() => Play(bow);

    public void PlaySiu() => Play(siu);

    public void PlayEnemyAttack() => Play(enemyAttack);

    public void PlayEnemyDie() => Play(enemyDie);

    public void PlayEnemyInjure() => Play(enemyInjure);

    public void PlayReward() => Play(reward);

    public void PlayQuestComplete() => Play(questComplete);

    public void PlayFailed() => Play(failed);

    private void Play(AudioClip clip)
    {
        if (source != null && clip != null) source.PlayOneShot(clip);
    }
}