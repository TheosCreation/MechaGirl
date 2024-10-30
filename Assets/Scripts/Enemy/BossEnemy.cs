using UnityEngine.AI;
using UnityEngine;

public class BossEnemy : Enemy
{
    public BossSpawner spawner;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        health = maxHealth;
        weapon = GetComponentInChildren<Weapon>();
        delayTimer = gameObject.AddComponent<Timer>();
        StateMachine = new EnemyStateMachineBuilder()
            .AddState(new BossAttackingState())
            .Build();

        SetDefaultState();
    }


    public override void EndAttack()
    {
        base.EndAttack();

        spawner.SpawnEnemies();
    }
}