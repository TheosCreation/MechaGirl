using UnityEngine.AI;
using UnityEngine;
using System;

public class BossEnemy : Enemy
{
    public BossSpawner spawner;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        health = maxHealth;
        weapons = GetComponentsInChildren<Weapon>();
        delayTimer = gameObject.AddComponent<Timer>();
        StateMachine = new EnemyStateMachineBuilder()
            .AddState(new IdleState())
            .AddState(new BossAttackingState())
            .Build();

        SetDefaultState();
    }


    public override void EndAttack()
    {
        base.EndAttack();

        spawner.SpawnEnemies();
    }

    //Removed the drop weapon on death
    public override void Die()
    {
        InvokeOnDeath();
        Destroy(gameObject);
    }

    public void SetActive(bool active)
    {
        if (active == true)
        {
            StateMachine.ChangeState(new BossAttackingState(), this);
            isInvinsable = false;
        }
        else
        {
            StateMachine.ChangeState(new IdleState(), this);
            isInvinsable = true;
        }
    }
}