using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActionWander : FSMAction
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float wanderTime;
    [SerializeField] private Vector2 moveRange;

    private Vector3 _movePosition;
    private float _timer;

    private void Start()
    {
        GetNewDestination();
    }

    public override void Act()
    {
        _timer -= Time.deltaTime;
        Vector3 moveDirection = (_movePosition - transform.position).normalized;
        Vector3 movement = moveDirection * (speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _movePosition) >= 0.5f)
        {
            transform.Translate(movement);
        }

        if (_timer <= 0f)
        {
            GetNewDestination();
            _timer = wanderTime;
        }
    }

    private void GetNewDestination()
    {
        float randomX = Random.Range(-moveRange.x,moveRange.x);
        float randomY = Random.Range(-moveRange.y,moveRange.y);
        _movePosition = transform.position + new Vector3(randomX, randomY);
    }

    private void OnDrawGizmosSelected()
    {
        if (moveRange != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, moveRange * 2f);
            Gizmos.DrawLine(transform.position,_movePosition);
        }
    }
}
