using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Config")] 
    [SerializeField] private float health;
    [SerializeField] private float experienceReward = 50f;
    [SerializeField] private float dropExperience = 10f;
    [SerializeField] private float dropHealthRestore = 0f;
    [SerializeField] private int goldReward = 5;
    
    public float CurrentHealth { get; private set; }

    private Animator _animator;
    private EnemyBrain _enemyBrain;
    private EnemySelector _enemySelector;
    private bool _isDead;

    public bool IsDead => _isDead;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyBrain = GetComponent<EnemyBrain>();
        _enemySelector = GetComponent<EnemySelector>();
    }

    public void Start()
    {
        InitializeForSpawn();
    }

    public void InitializeForSpawn()
    {
        _isDead = false;
        CurrentHealth = Mathf.Max(health, 1f);
        if (_enemyBrain != null) _enemyBrain.enabled = true;
        if (_animator != null) _animator.ResetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, false);
    }

    public void TakeDamage(float amount, bool isCritical)
    {
        if (_isDead || amount <= 0f) return;
        CurrentHealth -= amount;
        DamageManager.Instance?.ShowDamageText(amount, transform, isCritical);
        if (CurrentHealth <= 0f)
        {
            _isDead = true;
            CurrentHealth = 0f;
            if (_animator != null) _animator.SetTrigger("Dead");
            if (_enemyBrain != null) _enemyBrain.enabled = false;
            _enemySelector?.NoSelectionCallback();
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            PlayerExp playerExp = FindObjectOfType<PlayerExp>();
            playerExp?.AddExp(experienceReward);
            FindObjectOfType<QuestManager>()?.RegisterKill();
            SpawnDrop();
            GetComponentInParent<EnemyRespawner>()?.NotifyEnemyDefeated();
            StartCoroutine(DisableAfterDeath());
        }
    }

    private void SpawnDrop()
    {
        GameObject dropObject = new GameObject("EnemyDrop", typeof(EnemyDrop));
        dropObject.transform.position = transform.position;
        SpriteRenderer sourceRenderer = GetComponent<SpriteRenderer>();
        dropObject.GetComponent<EnemyDrop>().Initialize(dropExperience, dropHealthRestore, goldReward,
            sourceRenderer == null ? null : sourceRenderer.sprite);
    }

    private System.Collections.IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(0.75f);
        gameObject.SetActive(false);
    }
}
