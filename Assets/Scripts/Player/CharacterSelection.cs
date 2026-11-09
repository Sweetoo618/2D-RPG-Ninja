using UnityEngine;

/// <summary>跨场景持有玩家当前选择的角色。</summary>
public static class CharacterSelection
{
    public static CharacterDefinition Selected { get; set; }

    /// <summary>把当前选择的角色控制器套到指定 Animator 上。</summary>
    public static void Apply(Animator animator)
    {
        if (animator == null || Selected == null || Selected.animatorController == null) return;
        animator.runtimeAnimatorController = Selected.animatorController;
    }
}