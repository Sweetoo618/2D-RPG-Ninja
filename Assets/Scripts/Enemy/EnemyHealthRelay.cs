using UnityEngine;

public sealed class EnemyHealthRelay : MonoBehaviour
{
    private EnemyRespawner _respawner;
    private EnemyHealth _health;
    private bool _reported;

    public void Initialize(EnemyRespawner respawner) => _respawner = respawner;

    private void Awake() => _health = GetComponent<EnemyHealth>();

    private void Update()
    {
        if (!_reported && _health != null && _health.CurrentHealth <= 0f)
        {
            _reported = true;
            _respawner?.NotifyEnemyDefeated();
        }
    }
}
