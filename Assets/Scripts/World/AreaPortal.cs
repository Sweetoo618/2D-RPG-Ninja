using TMPro;
using UnityEngine;

/// <summary>
/// Small, reusable travel point used by the village, enemy and NPC areas.
/// The scene owns the destination data; the portal only handles proximity and input.
/// </summary>
public sealed class AreaPortal : MonoBehaviour
{
    [SerializeField] private string destinationScene;
    [SerializeField] private Vector3 arrivalPosition;
    [SerializeField] private string label = "E Travel";
    [SerializeField] private float interactionRadius = 1.25f;

    private Player _player;
    private TMP_Text _prompt;

    public void Configure(string scene, Vector3 arrival, string prompt)
    {
        destinationScene = scene;
        arrivalPosition = arrival;
        label = prompt;
    }

    private void Awake()
    {
        // Portal labels are optional; travel remains usable from the interaction radius.
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (_player == null) _player = FindObjectOfType<Player>();
        bool inRange = _player != null && Vector2.Distance(transform.position, _player.transform.position) <= interactionRadius;
        if (_prompt != null) _prompt.gameObject.SetActive(inRange && !DialogueManager.IsOpen);
        if (inRange && !DialogueManager.IsOpen && Input.GetKeyDown(KeyCode.E))
            SceneTransition.Instance?.TravelTo(destinationScene, arrivalPosition);
    }
}
