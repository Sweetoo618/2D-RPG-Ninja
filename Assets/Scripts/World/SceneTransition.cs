using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneTransition : MonoBehaviour
{
    private static bool _hasPendingSceneRestore;
    private static Vector3 _pendingPosition;
    private static string _pendingActiveQuestId;
    private static int _pendingQuestProgress;
    private static string[] _pendingCompletedQuestIds;

    [SerializeField] private PlayerStats stats;
    private Player _player;
    private QuestManager _questManager;
    private PlayerInventory _inventory;
    private bool _autoLoaded;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        _hasPendingSceneRestore = false;
        _pendingActiveQuestId = null;
        _pendingQuestProgress = 0;
        _pendingCompletedQuestIds = null;
    }

    private void Update()
    {
        if (_autoLoaded || _hasPendingSceneRestore) return;
        _autoLoaded = true;
        if (SaveSystem.HasSave) LoadGame();
    }

    public void TravelTo(string sceneName, Vector3 arrivalPosition)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        _pendingPosition = arrivalPosition;
        _hasPendingSceneRestore = true;
        LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        SaveSystem.Save(stats, _player == null ? null : _player.transform, _questManager, _inventory);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame()
    {
        if (!SaveSystem.Load(stats, out string scene, out Vector3 position, out string activeQuestId, out int questProgress, out string[] completedQuestIds, _inventory)) return;
        if (!string.IsNullOrEmpty(scene) && scene != SceneManager.GetActiveScene().name)
        {
            _pendingPosition = position;
            _pendingActiveQuestId = activeQuestId;
            _pendingQuestProgress = questProgress;
            _pendingCompletedQuestIds = completedQuestIds;
            _hasPendingSceneRestore = true;
            SceneManager.LoadSceneAsync(scene);
            return;
        }
        Restore(position, activeQuestId, questProgress, completedQuestIds);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (!_hasPendingSceneRestore) return;
        _hasPendingSceneRestore = false;
        _autoLoaded = true;
        StartCoroutine(RestoreAfterSceneLoad(_pendingPosition, _pendingActiveQuestId, _pendingQuestProgress, _pendingCompletedQuestIds));
    }

    private IEnumerator RestoreAfterSceneLoad(Vector3 position, string activeQuestId, int questProgress, string[] completedQuestIds)
    {
        yield return null;
        Restore(position, activeQuestId, questProgress, completedQuestIds);
    }

    private void Restore(Vector3 position, string activeQuestId, int questProgress, string[] completedQuestIds)
    {
        _player = FindObjectOfType<Player>();
        if (_player != null)
        {
            _player.transform.position = position;
            CharacterSelection.Apply(_player.GetComponent<Animator>());
        }
        PlayerIconUI iconUI = FindObjectOfType<PlayerIconUI>();
        if (iconUI != null) iconUI.Apply();
        _questManager = FindObjectOfType<QuestManager>();
        if (_questManager != null) _questManager.RestoreState(activeQuestId, questProgress, completedQuestIds);
        _inventory = FindObjectOfType<PlayerInventory>();
        SaveSystem.ApplyPendingInventory(_inventory);
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save(stats, _player == null ? null : _player.transform, _questManager, _inventory);
        _hasPendingSceneRestore = false;
    }
}