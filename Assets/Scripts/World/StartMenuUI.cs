using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 开始场景菜单：选择单人/多人模式与 New Game / Load Game，点击开始后异步加载游戏场景。
/// </summary>
public sealed class StartMenuUI : MonoBehaviour
{
    private const int SingleMode = 0;
    private const int NewGameOption = 0;

    [Header("Mode")]
    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private GameObject singlePlayerGroup;
    [SerializeField] private GameObject multiPlayerGroup;

    [Header("Select")]
    [SerializeField] private TMP_Dropdown singleSelectDropdown;

    [Header("Start")]
    [SerializeField] private Button startButton;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private string gameSceneName = "GameScene";

    private bool _isLoading;

    private void Awake()
    {
        if (modeDropdown != null) modeDropdown.onValueChanged.AddListener(OnModeChanged);
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
    }

    private void Start()
    {
        OnModeChanged(modeDropdown != null ? modeDropdown.value : SingleMode);
    }

    private void OnDestroy()
    {
        if (modeDropdown != null) modeDropdown.onValueChanged.RemoveListener(OnModeChanged);
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
    }

    private void OnModeChanged(int mode)
    {
        bool single = mode == SingleMode;
        if (singlePlayerGroup != null) singlePlayerGroup.SetActive(single);
        if (multiPlayerGroup != null) multiPlayerGroup.SetActive(!single);
        if (startButton != null) startButton.interactable = single;
    }

    private void OnStartClicked()
    {
        if (_isLoading) return;
        if (modeDropdown != null && modeDropdown.value != SingleMode) return;

        bool loadGame = singleSelectDropdown != null && singleSelectDropdown.value != NewGameOption;
        if (loadGame)
        {
            // 无存档时不允许进入游戏
            if (!SaveSystem.HasSave) return;
        }
        else
        {
            ResetForNewGame();
        }
        LoadGameScene();
    }

    private void ResetForNewGame()
    {
        if (playerStats != null) playerStats.ResetPlayer();
        SaveSystem.DeleteSave();
    }

    private void LoadGameScene()
    {
        if (_isLoading || string.IsNullOrWhiteSpace(gameSceneName)) return;
        _isLoading = true;
        if (startButton != null) startButton.interactable = false;
        SceneManager.LoadSceneAsync(gameSceneName);
    }
}