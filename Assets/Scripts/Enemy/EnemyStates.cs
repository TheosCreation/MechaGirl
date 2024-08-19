using UnityEngine;
using UnityEngine.AI;

// Enum for possible enemy states for debugging purposes mostly
public enum EnemyState
{
    Looking,
    Chasing,
    Attacking,
    NOTIMPLEMENTED
}
public interface IEnemyState
{
    void Enter(Enemy enemy);
    void Execute(Enemy enemy);
    void Exit(Enemy enemy);
}

public class LookingState : IEnemyState
{
    //probably dont even need wander but good to have just in case
    private float wanderRadius = 5f;
    private float wanderTimer = 3f;
    private float timer;

    public void Enter(Enemy enemy)
    {
        timer = wanderTimer;
    }

    public void Execute(Enemy enemy)
    {
        timer += Time.deltaTime;

        if (enemy.IsTargetInFOV())
        {
            enemy.StateMachine.ChangeState(new ChaseState(), enemy);
            return;
        }

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(enemy.transform.position, wanderRadius, -1);
            enemy.agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public void Exit(Enemy enemy)
    {
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}

public class ChaseState : IEnemyState
{
    float timer = 0.0f;
    public void Enter(Enemy enemy)
    {
        enemy.agent.SetDestination(enemy.target.position);
        enemy.animator.SetBool("IsMoving", true);
    }

    public void Execute(Enemy enemy)
    {
        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (dist < enemy.attackDistance)
        {
            enemy.StateMachine.ChangeState(new AttackingState(), enemy);
        }
        else if (dist > enemy.lookDistance)
        {
            enemy.StateMachine.ChangeState(new LookingState(), enemy);
        }

        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            timer = enemy.updatePathTime;
            enemy.agent.SetDestination(enemy.target.position);
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}

public class AttackingState : IEnemyState
{
    private bool attackTriggered = false;
    private float attackStartTime;
    private float rotationResumeTime;

    public void Enter(Enemy enemy)
    {
        // Start attack delay and animation
        enemy.agent.isStopped = true;
        enemy.canRotate = false;
        enemy.delayTimer.SetTimer(enemy.attackStartDelay, enemy.StartAttack);

        enemy.weapon.OnAttack += () => AttackExecuted(enemy);
        attackTriggered = true;
        attackStartTime = Time.time;

        // Stop rotation for a second after attack is triggered
        rotationResumeTime = Time.time + enemy.attackStartDelay + enemy.attackResumeRotationDelay;
    }

    private void AttackExecuted(Enemy enemy)
    {
        enemy.canRotate = false;
        rotationResumeTime = Time.time + enemy.attackResumeRotationDelay;
    }

    public void Execute(Enemy enemy)
    {
        if (Time.time >= rotationResumeTime)
        {
            enemy.canRotate = true;
        }

        // Check distance to target
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (attackTriggered && Time.time >= attackStartTime + enemy.attackDuration)
        {
            if (distanceToTarget >= enemy.loseDistance)
            {
                // Switch back to chasing if the target is out of sight
                enemy.StateMachine.ChangeState(new ChaseState(), enemy);
            }
        }
    }

    public void Exit(Enemy enemy)
    {
        // Resume movement or other logic as needed
        enemy.agent.isStopped = false;
        enemy.delayTimer.StopTimer();
        enemy.EndAttack();
        enemy.weapon.OnAttack -= () => AttackExecuted(enemy);
    }
}