using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>UGUI 点击音效：任意可交互 UGUI 控件（Button、Dropdown 等）被按下时播放一次。</summary>
public sealed class UGUIClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private readonly List<RaycastResult> _results = new List<RaycastResult>();
    private float _baseVolume;

    private void Awake()
    {
        if (source == null) return;
        _baseVolume = source.volume;
        ApplyVolume();
        GameAudioSettings.Changed += ApplyVolume;
    }

    private void OnDestroy() => GameAudioSettings.Changed -= ApplyVolume;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) PlayIfOnSelectable(Input.mousePosition);
    }

    private void ApplyVolume()
    {
        if (source != null) source.volume = _baseVolume * GameAudioSettings.SoundVolume;
    }

    private void PlayIfOnSelectable(Vector2 screenPosition)
    {
        if (EventSystem.current == null || source == null || clip == null) return;

        PointerEventData pointer = new PointerEventData(EventSystem.current) { position = screenPosition };
        _results.Clear();
        EventSystem.current.RaycastAll(pointer, _results);
        if (_results.Count == 0) return;

        Selectable target = _results[0].gameObject.GetComponentInParent<Selectable>();
        if (target == null || !target.IsInteractable()) return;
        source.PlayOneShot(clip);
    }
}