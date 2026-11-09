using UnityEngine;
using UnityEngine.UI;

/// <summary>玩家头像：按当前选择角色显示对应的 Faceset。</summary>
public sealed class PlayerIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    private void OnEnable() => Apply();

    public void Apply()
    {
        CharacterDefinition character = CharacterSelection.Selected;
        if (icon == null || character == null || character.portrait == null) return;
        icon.sprite = character.portrait;
    }
}