using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private readonly int _moveX = Animator.StringToHash("MoveX");
    private readonly int _moveY = Animator.StringToHash("MoveY");
    private readonly int _moving = Animator.StringToHash("Moving");
    private readonly int _dead = Animator.StringToHash("Dead");
    private readonly int _revive = Animator.StringToHash("Revive");
    private readonly int _attacking = Animator.StringToHash("Attacking");
    
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetDeadAnimation()
    {
        _animator.SetTrigger(_dead);
    }

    public void SetMoveBoolTransition(bool value)
    {
        _animator.SetBool(_moving, value);
    }

    public void SetMoveAnimation(Vector2 dir)
    {
        _animator.SetFloat(_moveX, dir.x);
        _animator.SetFloat(_moveY, dir.y);
    }

    public void SetAttackAnimation(bool value)
    {
        _animator.SetBool(_attacking, value);
    }

    public void ResetPlayer()
    {
        SetMoveAnimation(Vector2.down);
        _animator.SetTrigger(_revive);
    }
}
