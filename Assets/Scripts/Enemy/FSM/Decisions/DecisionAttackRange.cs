using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(EnemyBrain))]
public class DecisionAttackRange : FSMDecision
{
    [FormerlySerializedAs("range")]
    [Header("Config")]
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerMask;

    private EnemyBrain _enemy;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBrain>();
    }

    public override bool Decide()
    {
        return PlayerInAttackRange();
    }

    private bool PlayerInAttackRange()
    {
        if (_enemy == null || _enemy.Player == null) return false;
        return _enemy.IsPlayerWithin(attackRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
