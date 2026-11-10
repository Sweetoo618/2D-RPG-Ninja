using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>鼠标在音量条上左右拖动调节音量。</summary>
public sealed class AudioVolumeBar : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum Channel
    {
        Bgm,
        Sound
    }

    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI amountTMP;
    [SerializeField] private Channel channel;

    private void OnEnable() => Refresh();

    public void OnPointerDown(PointerEventData eventData) => SetFromPointer(eventData);

    public void OnDrag(PointerEventData eventData) => SetFromPointer(eventData);

    public void OnPointerUp(PointerEventData eventData) => GameAudioSettings.Save();

    private void SetFromPointer(PointerEventData eventData)
    {
        RectTransform rect = fill.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) return;
        float value = Mathf.Clamp01((localPoint.x - rect.rect.xMin) / rect.rect.width);
        Apply(value);
        if (channel == Channel.Bgm) GameAudioSettings.SetBgmVolume(value);
        else GameAudioSettings.SetSoundVolume(value);
    }

    private void Refresh()
    {
        Apply(channel == Channel.Bgm ? GameAudioSettings.BgmVolume : GameAudioSettings.SoundVolume);
    }

    private void Apply(float value)
    {
        fill.fillAmount = value;
        amountTMP.text = $"{Mathf.RoundToInt(value * 100f)}/100";
    }
}
