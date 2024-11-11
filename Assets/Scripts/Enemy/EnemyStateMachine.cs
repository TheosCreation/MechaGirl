using UnityEngine;

public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState newState, Enemy enemy)
    {
        if (currentState == newState)
        {
            Debug.Log("state unchanged" + newState);
            return;

        }
        currentState?.Exit(enemy);
        currentState = newState;
        currentState.Enter(enemy);
    }

    public void Update(Enemy enemy)
    {
        currentState?.Execute(enemy);
    }

    public EnemyState GetCurrentState()
    {
        // Determine the current state based on the state machine
        if (currentState is LookingState)
            return EnemyState.Looking;
        else if (currentState is ChaseState)
            return EnemyState.Chasing;
        else if (currentState is AttackingState)
            return EnemyState.Attacking;
        else if (currentState is BossAttackingState)
            return EnemyState.BossAttacking;
        else if (currentState is FlyingWonderState)
            return EnemyState.FlyingWonder;
        else if (currentState is FlyingAttackingState)
            return EnemyState.FlyingAttacking;
        else if (currentState is IdleState)
            return EnemyState.Idle;
        else
            return EnemyState.NOTIMPLEMENTED; // Default
    }
}
public class EnemyStateMachineBuilder
{
    private EnemyStateMachine stateMachine;

    public EnemyStateMachineBuilder()
    {
        stateMachine = new EnemyStateMachine();
    }

    public EnemyStateMachineBuilder AddState(IEnemyState state)
    {
        return this;
    }

    public EnemyStateMachine Build()
    {
        return stateMachine;
    }
}
