using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneTransition : MonoBehaviour
{
    private static bool _hasPendingSceneRestore;
    private static Vector3 _pendingPosition;
    private static int _pendingQuestProgress;
    private static bool _pendingQuestAccepted;
    private static bool _pendingQuestRewardClaimed;

    [SerializeField] private PlayerStats stats;
    private Player _player;
    private QuestManager _questManager;
    private PlayerInventory _inventory;

    public static SceneTransition Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (stats == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null) stats = player.Stats;
        }
        _player = FindObjectOfType<Player>();
        _questManager = FindObjectOfType<QuestManager>();
        _inventory = FindObjectOfType<PlayerInventory>();

        if (_hasPendingSceneRestore)
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
    }

    public void TravelTo(string sceneName, Vector3 arrivalPosition)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        _pendingPosition = arrivalPosition;
        _hasPendingSceneRestore = true;
        LoadScene(sceneName);
    }

    private void Update()
    {
        if (DialogueManager.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.F5)) SaveSystem.Save(stats, _player == null ? null : _player.transform, _questManager, _inventory);
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        SaveSystem.Save(stats, _player == null ? null : _player.transform, _questManager, _inventory);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame()
    {
        if (!SaveSystem.Load(stats, out string scene, out Vector3 position, out int questProgress, out bool questAccepted, out bool questRewardClaimed)) return;
        if (!string.IsNullOrEmpty(scene) && scene != SceneManager.GetActiveScene().name)
        {
            _pendingPosition = position;
            _pendingQuestProgress = questProgress;
            _pendingQuestAccepted = questAccepted;
            _pendingQuestRewardClaimed = questRewardClaimed;
            _hasPendingSceneRestore = true;
            SceneManager.LoadSceneAsync(scene);
            return;
        }
        Restore(position, questProgress, questAccepted, questRewardClaimed);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (!_hasPendingSceneRestore) return;
        _hasPendingSceneRestore = false;
        StartCoroutine(RestoreAfterSceneLoad(_pendingPosition, _pendingQuestProgress, _pendingQuestAccepted, _pendingQuestRewardClaimed));
    }

    private System.Collections.IEnumerator RestoreAfterSceneLoad(Vector3 position, int questProgress, bool questAccepted, bool questRewardClaimed)
    {
        // Let the new scene finish Awake/Start before replacing its starter inventory.
        yield return null;
        Restore(position, questProgress, questAccepted, questRewardClaimed);
    }

    private void Restore(Vector3 position, int questProgress, bool questAccepted, bool questRewardClaimed)
    {
        _player = FindObjectOfType<Player>();
        if (_player != null) _player.transform.position = position;
        _questManager = FindObjectOfType<QuestManager>();
        if (_questManager != null) _questManager.RestoreState(questProgress, questAccepted, questRewardClaimed);
        _inventory = FindObjectOfType<PlayerInventory>();
        SaveSystem.ApplyPendingInventory(_inventory);
    }
}
