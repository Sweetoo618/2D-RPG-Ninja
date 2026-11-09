using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>开始场景角色选择：左右切换角色，并用该角色的移动帧做动画预览。</summary>
public sealed class CharacterSelectionUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterDefinition[] characters;

    [Header("Refs")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image preview;
    [SerializeField] private TMP_Text nameText;

    private int _index;
    private Coroutine _previewRoutine;

    private void Awake()
    {
        if (previousButton != null) previousButton.onClick.AddListener(Previous);
        if (nextButton != null) nextButton.onClick.AddListener(Next);
    }

    private void OnEnable()
    {
        if (characters == null || characters.Length == 0) return;
        _index = Mathf.Clamp(_index, 0, characters.Length - 1);
        ShowCurrent();
    }

    private void OnDestroy()
    {
        if (previousButton != null) previousButton.onClick.RemoveListener(Previous);
        if (nextButton != null) nextButton.onClick.RemoveListener(Next);
    }

    private void Previous()
    {
        if (characters == null || characters.Length == 0) return;
        _index = (_index - 1 + characters.Length) % characters.Length;
        ShowCurrent();
    }

    private void Next()
    {
        if (characters == null || characters.Length == 0) return;
        _index = (_index + 1) % characters.Length;
        ShowCurrent();
    }

    private void ShowCurrent()
    {
        CharacterDefinition current = characters[_index];
        CharacterSelection.Selected = current;
        if (nameText != null) nameText.text = current != null ? current.displayName : string.Empty;
        if (preview != null) preview.preserveAspect = true;
        RestartPreview(current);
    }

    private void RestartPreview(CharacterDefinition current)
    {
        if (_previewRoutine != null) StopCoroutine(_previewRoutine);
        if (preview == null || current == null) return;
        if (current.previewFrames != null && current.previewFrames.Length > 1)
            _previewRoutine = StartCoroutine(CyclePreview(current.previewFrames));
        else if (current.previewSprite != null)
            preview.sprite = current.previewSprite;
    }

    private IEnumerator CyclePreview(Sprite[] frames)
    {
        int i = 0;
        while (true)
        {
            if (preview != null) preview.sprite = frames[i % frames.Length];
            i++;
            yield return new WaitForSeconds(0.18f);
        }
    }
}