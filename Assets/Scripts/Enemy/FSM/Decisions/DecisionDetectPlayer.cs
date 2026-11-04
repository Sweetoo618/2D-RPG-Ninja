using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionDetectPlayer : FSMDecision
{
    [Header("Config")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask playerMask;

    private EnemyBrain _enemy;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBrain>();
    }

    public override bool Decide()
    {
        return DetectPlayer();
    }

    private bool DetectPlayer()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && Vector2.Distance(_enemy.transform.position, player.transform.position) <= range)
        {
            _enemy.Player = player.transform;
            return true;
        }

        _enemy.Player = null;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
