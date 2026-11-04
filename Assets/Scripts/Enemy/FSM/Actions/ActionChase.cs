using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class ActionChase : FSMAction
{
    [Header("Config")] 
    [SerializeField] private float chaseSpeed;

    private EnemyBrain _enemyBrain;

    private void Awake()
    {
        _enemyBrain = GetComponent<EnemyBrain>();
    }

    public override void Act()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (_enemyBrain == null || _enemyBrain.Player == null) return;
        Vector3 dirToPlayer = _enemyBrain.Player.position - transform.position;
        if (dirToPlayer.sqrMagnitude <= 0.0001f) return;
        // The attack-range decision owns the inner radius. Chase only moves toward the target.
        transform.position += dirToPlayer.normalized * (chaseSpeed * Time.deltaTime);
    }
}
