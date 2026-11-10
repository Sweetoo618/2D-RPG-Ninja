using System;
using System.IO;
using UnityEngine;

/// <summary>BGM 与音效音量设置，独立于游戏存档保存。</summary>
public static class GameAudioSettings
{
    [Serializable]
    private class Data
    {
        public float bgmVolume = 1f;
        public float soundVolume = 1f;
    }

    public static event Action Changed;

    private static Data _data;

    private static string SavePath => Path.Combine(Application.persistentDataPath, "settings.json");

    public static float BgmVolume
    {
        get
        {
            EnsureLoaded();
            return _data.bgmVolume;
        }
    }

    public static float SoundVolume
    {
        get
        {
            EnsureLoaded();
            return _data.soundVolume;
        }
    }

    public static void SetBgmVolume(float value)
    {
        EnsureLoaded();
        _data.bgmVolume = Mathf.Clamp01(value);
        Changed?.Invoke();
    }

    public static void SetSoundVolume(float value)
    {
        EnsureLoaded();
        _data.soundVolume = Mathf.Clamp01(value);
        Changed?.Invoke();
    }

    public static void Save()
    {
        EnsureLoaded();
        File.WriteAllText(SavePath, JsonUtility.ToJson(_data, true));
    }

    private static void EnsureLoaded()
    {
        if (_data != null) return;
        _data = File.Exists(SavePath) ? JsonUtility.FromJson<Data>(File.ReadAllText(SavePath)) : null;
        if (_data == null) _data = new Data();
        _data.bgmVolume = Mathf.Clamp01(_data.bgmVolume);
        _data.soundVolume = Mathf.Clamp01(_data.soundVolume);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        _data = null;
        Changed = null;
    }
}
