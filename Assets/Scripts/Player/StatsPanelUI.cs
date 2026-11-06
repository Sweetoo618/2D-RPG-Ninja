using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家属性面板（Stats-Panel）视图/控制器。
/// 读取 PlayerStats 数据，刷新 Stats-Data-BK 的 8 项数值与 Points-Text 的点数，
/// 并把 Points-Data-BK 中四个「+」按钮接到属性加点。
/// 
/// 层级约定（相对本脚本所在的 Stats-Panel）：
///   Stats-Data-BK / &lt;Part&gt; / Number    —— 8 项：Level/Health/Mana/Exp/Strength/Vitality/Intelligence/Defense
///   Points-Text   / Number                 —— 当前剩余属性点
///   Points-Data-BK / &lt;Part&gt; / Number + Add-Button —— 4 项：Strength/Vitality/Intelligence/Defense
/// </summary>
public sealed class StatsPanelUI : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Player player;

    // Stats-Data-BK 的 8 项数值
    private TMP_Text _level;
    private TMP_Text _health;
    private TMP_Text _mana;
    private TMP_Text _exp;
    private TMP_Text _strength;
    private TMP_Text _vitality;
    private TMP_Text _intelligence;
    private TMP_Text _defense;

    // Points-Text 的剩余点数
    private TMP_Text _points;

    // Points-Data-BK 的 4 项数值与加点按钮
    private TMP_Text _strValue;
    private TMP_Text _vitValue;
    private TMP_Text _intValue;
    private TMP_Text _defValue;
    private Button _strButton;
    private Button _vitButton;
    private Button _intButton;
    private Button _defButton;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<Player>();
        Bind();

        _strButton?.onClick.AddListener(() => Spend(PlayerStats.AttributeType.Strength));
        _vitButton?.onClick.AddListener(() => Spend(PlayerStats.AttributeType.Vitality));
        _intButton?.onClick.AddListener(() => Spend(PlayerStats.AttributeType.Intelligence));
        _defButton?.onClick.AddListener(() => Spend(PlayerStats.AttributeType.Defense));
    }

    private void OnEnable() => Refresh();

    private void Update() => Refresh();

    private void Bind()
    {
        Transform data = transform.Find("Stats-Data-BK");
        _level = FindNumber(data, "Level-Part");
        _health = FindNumber(data, "Health-Part");
        _mana = FindNumber(data, "Mana-Part");
        _exp = FindNumber(data, "Exp-Part");
        _strength = FindNumber(data, "Strength-Part");
        _vitality = FindNumber(data, "Vitality-Part");
        _intelligence = FindNumber(data, "Intelligence-Part");
        _defense = FindNumber(data, "Defense-Part");

        _points = FindNumber(transform.Find("Points-Text"), null);

        Transform pointsData = transform.Find("Points-Data-BK");
        _strValue = FindNumber(pointsData, "Strength-Part");
        _strButton = FindAddButton(pointsData, "Strength-Part");
        _vitValue = FindNumber(pointsData, "Vitality-Part");
        _vitButton = FindAddButton(pointsData, "Vitality-Part");
        _intValue = FindNumber(pointsData, "Intelligence-Part");
        _intButton = FindAddButton(pointsData, "Intelligence-Part");
        _defValue = FindNumber(pointsData, "Defense-Part");
        _defButton = FindAddButton(pointsData, "Defense-Part");
    }

    private static TMP_Text FindNumber(Transform parent, string partName)
    {
        if (parent == null) return null;
        Transform part = partName == null ? parent : parent.Find(partName);
        if (part == null) return null;
        Transform number = part.Find("Number");
        return number == null ? null : number.GetComponent<TMP_Text>();
    }

    private static Button FindAddButton(Transform parent, string partName)
    {
        if (parent == null) return null;
        Transform part = parent.Find(partName);
        if (part == null) return null;
        Transform button = part.Find("Add-Button");
        return button == null ? null : button.GetComponent<Button>();
    }

    private void Spend(PlayerStats.AttributeType attribute)
    {
        PlayerStats stats = player == null ? null : player.Stats;
        if (stats == null) return;
        stats.SpendAttributePoint(attribute);
        Refresh();
    }

    private void Refresh()
    {
        PlayerStats stats = player == null ? null : player.Stats;
        if (stats == null) return;

        SetText(_level, stats.Level.ToString());
        SetText(_health, stats.Health.ToString("0"));
        SetText(_mana, stats.Mana.ToString("0"));
        SetText(_exp, stats.CurrentExp.ToString("0"));
        SetText(_strength, stats.Strength.ToString());
        SetText(_vitality, stats.Vitality.ToString());
        SetText(_intelligence, stats.Intelligence.ToString());
        SetText(_defense, stats.Defense.ToString());

        SetText(_points, stats.AttributePoints.ToString());

        SetText(_strValue, stats.Strength.ToString());
        SetText(_vitValue, stats.Vitality.ToString());
        SetText(_intValue, stats.Intelligence.ToString());
        SetText(_defValue, stats.Defense.ToString());
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text == null || text.text == value) return;
        text.text = value;
    }
}