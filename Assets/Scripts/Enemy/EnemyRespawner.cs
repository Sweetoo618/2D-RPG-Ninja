using System.Collections;
using UnityEngine;

public sealed class EnemyRespawner : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private int maxAlive = 5;

    // 阴影 30 秒消失后再等 1 分钟复活
    [SerializeField] private float respawnDelay = 90f;
    [SerializeField] private Vector2 spawnArea = new Vector2(8f, 5f);

    private int _alive;
    private EnemyHealth _template;

    private void Start()
    {
        EnemyHealth[] existing = FindObjectsOfType<EnemyHealth>();
        for (int i = 0; i < existing.Length; i++)
        {
            if (!existing[i].transform.IsChildOf(transform)) continue;
            if (_template == null) _template = existing[i];
            if (!existing[i].IsDead && existing[i].gameObject.activeInHierarchy) _alive++;
        }
    }

    public void NotifyEnemyDefeated()
    {
        _alive = Mathf.Max(0, _alive - 1);
        if (_alive < maxAlive && enemyPrefab != null) StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (_alive >= maxAlive) yield break;
        Vector2 offset = new Vector2(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y));
        EnemyHealth source = enemyPrefab != null ? enemyPrefab : _template;
        if (source == null) yield break;
        bool restoreSourceInactive = !source.gameObject.activeSelf;
        if (restoreSourceInactive) source.gameObject.SetActive(true);
        EnemyHealth enemy = Instantiate(source, transform.position + (Vector3)offset, Quaternion.identity, transform);
        if (restoreSourceInactive) source.gameObject.SetActive(false);
        enemy.InitializeForSpawn();
        _alive++;
    }
}
