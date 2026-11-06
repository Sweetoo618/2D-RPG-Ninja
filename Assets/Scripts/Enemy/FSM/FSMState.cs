using System;

[Serializable]
public class FSMState
{
    public string ID;
    public FSMAction[] Actions;
    public FSMTransition[] Transitions;

    public void UpdateState(EnemyBrain enemyBrain)
    {
        ExecuteActions();
        ExecuteTransitions(enemyBrain);
    }
    
    private void ExecuteActions()
    {
        if (Actions == null) return;
        for (int i = 0; i < Actions.Length; i++)
        {
            if (Actions[i] != null) Actions[i].Act();
        }
    }

    private void ExecuteTransitions(EnemyBrain enemyBrain)
    {
        if (Transitions == null || Transitions.Length <= 0) return;
        for (int i = 0; i < Transitions.Length; i++)
        {
            bool value = Transitions[i].Decision.Decide();
            // A state may have multiple transitions. The first matching transition
            // owns the frame; continuing would let later false branches overwrite it.
            string target = value ? Transitions[i].TrueState : Transitions[i].FalseState;
            if (string.IsNullOrEmpty(target)) continue;
            enemyBrain.ChangeState(target);
            return;
        }
    }
}



