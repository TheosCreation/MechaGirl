using UnityEngine.AI;
using UnityEngine;
using System;

public class BossEnemy : Enemy
{
    public BossSpawner spawner;
    public float spawnDelay = 0.5f; // for the spawner to slow down spawning

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

        spawner.StartSpawningEnemies();
    }

    //Removed the drop weapon on death
    public override void Die()
    {
        InvokeOnDeath();
        Instantiate(deathParticles, transform.position + new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
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