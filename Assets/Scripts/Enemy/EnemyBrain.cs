using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private string initState;
    [SerializeField] private FSMState[] states;
    public FSMState CurrentState { get; set; }
    public Transform Player { get; set; }

    public bool HasPlayer => Player != null;

    public bool IsPlayerWithin(float radius)
    {
        if (Player == null) return false;
        return Vector2.Distance(transform.position, Player.position) <= Mathf.Max(0f, radius);
    }
    
    private void Start()
    {
        ChangeState(initState);
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void ChangeState(string newStateID)
    {
        FSMState newState = GetState(newStateID);
        if (newState == null) return;
        CurrentState = newState;
    }

    private FSMState GetState(string newStateID)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].ID == newStateID)
            {
                return states[i];
            }
        }

        return null;
    }
}
