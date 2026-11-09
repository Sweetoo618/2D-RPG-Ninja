using UnityEngine;

/// <summary>
/// 可选角色数据：显示名、预览图与运行时 Animator 控制器。
/// </summary>
[CreateAssetMenu(menuName = "Game/Character", fileName = "Character_")]
public sealed class CharacterDefinition : ScriptableObject
{
    public string displayName;
    public Sprite previewSprite;
    public Sprite[] previewFrames;
    public Sprite portrait;
    public RuntimeAnimatorController animatorController;
}