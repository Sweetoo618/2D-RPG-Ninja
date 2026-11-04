using UnityEngine;

public sealed class AttributePointUI : MonoBehaviour
{
    private Player _player;

    private void Awake() => _player = FindObjectOfType<Player>();

    private void Update()
    {
        if (_player == null || _player.Stats == null || DialogueManager.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.F1)) 
            _player.Stats.SpendAttributePoint(PlayerStats.AttributeType.Strength);
        if (Input.GetKeyDown(KeyCode.F2)) 
            _player.Stats.SpendAttributePoint(PlayerStats.AttributeType.Vitality);
        if (Input.GetKeyDown(KeyCode.F3)) 
            _player.Stats.SpendAttributePoint(PlayerStats.AttributeType.Intelligence);
        if (Input.GetKeyDown(KeyCode.F4)) 
            _player.Stats.SpendAttributePoint(PlayerStats.AttributeType.Defense);
    }
}
