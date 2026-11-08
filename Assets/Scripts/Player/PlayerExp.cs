using UnityEngine;

public class PlayerExp : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    public void AddExp(float amount)
    {
        if (stats == null || amount <= 0f) return;
        if (stats.NextLevelExp <= 0f)
        {
            stats.NextLevelExp = Mathf.Max(stats.InitialNextLevelExp, 1f);
        }
        stats.CurrentExp += amount;
        while (stats.CurrentExp >= stats.NextLevelExp)
        {
            stats.CurrentExp -= stats.NextLevelExp;
            NextLevel();
        }
    }

    private void NextLevel()
    {
        stats.Level++;
        stats.AttributePoints += 3;
        float currentExpRequired = stats.NextLevelExp;
        float newNextLevelExp =
            Mathf.Round(currentExpRequired + stats.NextLevelExp
                * (stats.ExpMultiplier / 100f));
        stats.NextLevelExp = Mathf.Max(newNextLevelExp, currentExpRequired + 1f);
    }

    public int AvailableAttributePoints => stats == null ? 0 : stats.AttributePoints;
}



