using System.Collections;
using UnityEngine;

/// <summary>敌人死亡后延迟重生，重生位置为死亡时的位置。</summary>
public sealed class EnemyRespawner : MonoBehaviour
{
    [SerializeField] private float respawnDelay;

    public void NotifyEnemyDefeated(EnemyHealth enemy)
    {
        if (enemy == null) return;
        StartCoroutine(RespawnAfterDelay(enemy, enemy.transform.position));
    }

    private IEnumerator RespawnAfterDelay(EnemyHealth enemy, Vector3 position)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (enemy == null) yield break;
        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        enemy.InitializeForSpawn();
    }
}
