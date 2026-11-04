using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static event Action<EnemyBrain> OnEnemySelectedEvent;
    public static event Action OnNoSelectionEvent;
    
    [Header("Config")]
    [SerializeField] private LayerMask enemyMask;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        SelectEnemy();
    }

    private void SelectEnemy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit =
                Physics2D.Raycast(
                    _mainCamera.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero, Mathf.Infinity, enemyMask);
            if (hit.collider != null)
            {
                EnemyBrain enemy = hit.collider.GetComponent<EnemyBrain>();
                if (enemy == null) return;
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth.CurrentHealth <= 0f) return;
                OnEnemySelectedEvent?.Invoke(enemy);
            }
            else
            {
                OnNoSelectionEvent?.Invoke();
            }
        }
    }
}










