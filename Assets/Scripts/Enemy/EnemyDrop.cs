using UnityEngine;

public sealed class EnemyDrop : MonoBehaviour
{
    [SerializeField] private float experience = 10f;
    [SerializeField] private float healthRestore;
    [SerializeField] private int gold;
    [SerializeField] private float pickupRadius = 0.8f;
    private SpriteRenderer _spriteRenderer;
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    public void Initialize(float experienceAmount, float healthAmount, Sprite icon)
    {
        Initialize(experienceAmount, healthAmount, 0, icon);
    }

    public void Initialize(float experienceAmount, float healthAmount, int goldAmount, Sprite icon)
    {
        experience = Mathf.Max(0f, experienceAmount);
        healthRestore = Mathf.Max(0f, healthAmount);
        gold = Mathf.Max(0, goldAmount);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if (icon != null) _spriteRenderer.sprite = icon;
        _spriteRenderer.color = new Color(1f, 0.85f, 0.2f, 1f);
        transform.localScale = Vector3.one * 0.45f;
    }

    private void Update()
    {
        Player player = _player;
        if (player == null || Vector2.Distance(transform.position, player.transform.position) > pickupRadius) return;
        PlayerExp exp = player.GetComponent<PlayerExp>();
        if (exp != null) exp.AddExp(experience);
        if (healthRestore > 0f && player.Stats != null)
            player.Stats.RestoreHealth(healthRestore);
        if (gold > 0 && player.Stats != null)
        {
            player.Stats.AddGold(gold);
            SfxManager.Instance?.PlayReward();
        }
        Destroy(gameObject);
    }
}
