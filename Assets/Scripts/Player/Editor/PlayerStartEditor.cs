using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PlayerStats))]
public class PlayerStartEditor : Editor
{
    private PlayerStats StatsTarget => target as PlayerStats;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset Game"))
        {
            StatsTarget.ResetPlayer();
            SaveSystem.DeleteSave();
        }
    }
}
