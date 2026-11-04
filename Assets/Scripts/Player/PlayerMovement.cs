using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    public Vector2 MoveDirection => _moveDirection;
    
    private PlayerAnimations _playerAnimations;
    private PlayerActions _actions;
    private Player _player;
    private Rigidbody2D _rb2D;
    private Vector2 _moveDirection;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        _actions = new PlayerActions();
        _rb2D = GetComponent<Rigidbody2D>();
        _playerAnimations = GetComponent<PlayerAnimations>();
    }
    
    void Update()
    {
        ReadMovement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_player.Stats.Health <= 0) return;
        _rb2D.MovePosition(_rb2D.position + _moveDirection * (speed * Time.fixedDeltaTime));
    }
    
    private void ReadMovement()
    {
        if (DialogueManager.IsOpen)
        {
            _moveDirection = Vector2.zero;
            _playerAnimations.SetMoveBoolTransition(false);
            return;
        }

        _moveDirection = _actions.Movement.Move.ReadValue<Vector2>().normalized;
        if (_moveDirection == Vector2.zero)
        {
            _playerAnimations.SetMoveBoolTransition(false);
            return;
        }

        _playerAnimations.SetMoveBoolTransition(true);
        _playerAnimations.SetMoveAnimation(_moveDirection);
    }
    
    private void OnEnable()
    {
        if (_actions == null) _actions = new PlayerActions();
        _actions.Enable();
    }

    private void OnDisable()
    {
        if (_actions != null) _actions.Disable();
    }
}
