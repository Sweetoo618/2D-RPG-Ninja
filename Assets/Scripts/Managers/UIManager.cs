using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;

    [Header("Bars")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image expBar;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI healthTMP;
    [SerializeField] private TextMeshProUGUI manaTMP;
    [SerializeField] private TextMeshProUGUI expTMP;
    [SerializeField] private TextMeshProUGUI goldTMP;

    private void Awake()
    {
        if (stats == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null) stats = player.Stats;
        }
    }

    private void Update()
    {
        UpdataPlayerUI();
    }

    private void UpdataPlayerUI()
    {
        if (stats == null || healthBar == null || manaBar == null || expBar == null ||
            levelTMP == null || healthTMP == null || manaTMP == null || expTMP == null) return;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount,
            stats.Health / stats.MaxHealth, 10f * Time.deltaTime);
        manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount,
            stats.Mana / stats.MaxMana, 10f * Time.deltaTime);
        expBar.fillAmount = Mathf.Lerp(expBar.fillAmount,
            stats.CurrentExp / stats.NextLevelExp, 10f * Time.deltaTime);
        
        levelTMP.text = $"Level {stats.Level}";
        healthTMP.text = $"{stats.Health}/{stats.MaxHealth}";
        manaTMP.text = $"{stats.Mana}/{stats.MaxMana}";
        expTMP.text = $"{stats.CurrentExp}/{stats.NextLevelExp}";
        if (goldTMP != null) goldTMP.text = stats.Gold.ToString();
    }
}
