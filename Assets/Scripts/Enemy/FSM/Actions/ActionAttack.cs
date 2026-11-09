using UnityEngine;

public class ActionAttack : FSMAction
{
    [Header("Config")]
    [SerializeField] private float damage;
    [SerializeField] private float timeBtwAttacks;
    [SerializeField] private float attackRange = 2f;

    private EnemyBrain _enemyBrain;
    private float _timer;

    private void Awake()
    {
        _enemyBrain = GetComponent<EnemyBrain>();
    }

    public override void Act()
    {
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (_enemyBrain.Player == null || !_enemyBrain.IsPlayerWithin(attackRange)) return;
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            IDamageable player =
                _enemyBrain.Player.GetComponentInParent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damage);
                SfxManager.Instance?.PlayEnemyAttack();
            }
            _timer = timeBtwAttacks;
        }
    }
}
