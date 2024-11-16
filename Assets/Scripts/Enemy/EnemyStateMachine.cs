using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyStateMachine
{
    private IEnemyState currentState;
    private Dictionary<EnemyState, IEnemyState> stateInstances = new Dictionary<EnemyState, IEnemyState>();

    public void RegisterState(EnemyState stateEnum, IEnemyState stateInstance)
    {
        stateInstances[stateEnum] = stateInstance;
    }

    public void ChangeState(EnemyState newState, Enemy enemy)
    {
        if (!stateInstances.ContainsKey(newState))
        {
            Debug.LogError($"State {newState} not registered.");
            return;
        }

        IEnemyState newStateInstance = stateInstances[newState];

        if (currentState == newStateInstance)
        {
            Debug.Log("State unchanged: " + newState);
            return;
        }

        currentState?.Exit(enemy);
        currentState = newStateInstance;
        currentState.Enter(enemy);
    }

    public void Update(Enemy enemy)
    {
        currentState?.Execute(enemy);
    }

    public EnemyState GetCurrentState()
    {
        foreach (var pair in stateInstances)
        {
            if (pair.Value == currentState)
                return pair.Key;
        }
        return EnemyState.NOTIMPLEMENTED;
    }
}

public class EnemyStateMachineBuilder
{
    private EnemyStateMachine stateMachine;

    public EnemyStateMachineBuilder()
    {
        stateMachine = new EnemyStateMachine();
    }

    public EnemyStateMachineBuilder AddState(IEnemyState stateInstance, EnemyState stateEnum)
    {
        stateMachine.RegisterState(stateEnum, stateInstance);
        return this;
    }

    public EnemyStateMachine Build()
    {
        return stateMachine;
    }
}