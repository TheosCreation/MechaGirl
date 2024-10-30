using UnityEngine;
using UnityEngine.AI;

// Enum for possible enemy states for debugging purposes mostly
public enum EnemyState
{
    Looking,
    Chasing,
    Attacking,
    BossAttacking,
    FlyingWonder,
    FlyingAttacking,
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

        if (enemy.IsTargetInFOV() )
        {
            if (enemy.target)
            {
                enemy.StateMachine.ChangeState(new ChaseState(), enemy);
                return;
            }
        }

        if (timer >= wanderTimer)
        {
            if (enemy.isLaunching) return;
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
        if (!enemy.isLaunching) enemy.agent.SetDestination(enemy.target.position);
        enemy.animator.SetBool("IsMoving", true);
    }

    public void Execute(Enemy enemy)
    {
        if (!enemy.target) return;
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
            if (!enemy.isLaunching) enemy.agent.SetDestination(enemy.target.position);
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}
public class FlyingWonderState : IEnemyState
{
    float timer = 0.0f;
    public void Enter(Enemy enemy)
    {
        enemy.animator.SetBool("IsMoving", true);
    }

    public void Execute(Enemy enemy)
    {


        float dist = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (dist < enemy.attackDistance)
        {
            enemy.StateMachine.ChangeState(new FlyingAttackingState(), enemy);
        }
        else if (dist > enemy.lookDistance)
        {
            enemy.StateMachine.ChangeState(new LookingState(), enemy);
        }

        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            timer = enemy.updatePathTime;


        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}
public class FlyingAttackingState : IEnemyState
{

    private float attackStartTime;
    private float rotationResumeTime;
    private int bulletsFired = 0;

    public void Enter(Enemy enemy)
    {
        // Start attack delay and animation
        enemy.delayTimer.SetTimer(enemy.attackStartDelay, enemy.StartAttack);

        enemy.weapon.OnAttack += () => AttackExecuted(enemy);
        attackStartTime = Time.time;
    }

    private void AttackExecuted(Enemy enemy)
    {


        bulletsFired++;
        if (bulletsFired >= enemy.bulletsPerBurst)
        {
            Vector3 randomDirection = Random.onUnitSphere;

            float distanceFromEnemy = Random.Range(enemy.minRadius, enemy.maxRadius);

            distanceFromEnemy = Mathf.Min(distanceFromEnemy, enemy.maxRadius);

            Vector3 randomSpot = enemy.transform.position + randomDirection * distanceFromEnemy;

            bulletsFired = 0;
            enemy.EndAttack();
        }
    }



    public void Execute(Enemy enemy)
    {

        Vector3 directionToTarget = enemy.target.position - enemy.weapon.transform.position;
        enemy.weapon.transform.rotation = Quaternion.LookRotation(directionToTarget);

        // Check distance to target
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (Time.time >= attackStartTime + enemy.attackDuration)
        {
            enemy.StartAttack();
        }
    }


    public void Exit(Enemy enemy)
    {
        enemy.delayTimer.StopTimer();
        enemy.EndAttack();
        enemy.weapon.OnAttack -= () => AttackExecuted(enemy);
    }
}

public class AttackingState : IEnemyState
{

    private float attackStartTime;
    private float rotationResumeTime;
    private int bulletsFired = 0;

    public void Enter(Enemy enemy)
    {
        // Start attack delay and animation
        enemy.delayTimer.SetTimer(enemy.attackStartDelay, enemy.StartAttack);

        enemy.weapon.OnAttack += () => AttackExecuted(enemy);
        attackStartTime = Time.time;
        if (!enemy.isLaunching && !enemy.agent.enabled) enemy.agent.isStopped = true;
    }

    private void AttackExecuted(Enemy enemy)
    {


        bulletsFired++;
        if (bulletsFired >= enemy.bulletsPerBurst)
        {
            Vector3 randomDirection = Random.onUnitSphere;

            float distanceFromEnemy = Random.Range(enemy.minRadius, enemy.maxRadius);

            distanceFromEnemy = Mathf.Min(distanceFromEnemy, enemy.maxRadius);

            Vector3 randomSpot = enemy.transform.position + randomDirection * distanceFromEnemy;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomSpot, out hit, distanceFromEnemy, NavMesh.AllAreas))
            {
                if (enemy.agent.isActiveAndEnabled)
                {
                    enemy.agent.isStopped = false;
                    enemy.agent.SetDestination(hit.position);
                }
            }
            bulletsFired = 0;
            enemy.EndAttack();
        }
    }



    public void Execute(Enemy enemy)
    {

        Vector3 directionToTarget = enemy.target.position - enemy.weapon.transform.position;
        enemy.weapon.transform.rotation = Quaternion.LookRotation(directionToTarget);

        // Check distance to target
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if (!enemy.agent.enabled) return;
        if (Time.time >= attackStartTime + enemy.attackDuration)
        {
            if (!enemy.agent.pathPending && distanceToTarget >= enemy.attackDistance)
            {
                // Switch back to chasing if the target is out of sight
                enemy.StateMachine.ChangeState(new ChaseState(), enemy);
            }
            else if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            {
                // Allow shooting again if the enemy has reached the destination
                enemy.StartAttack();
                enemy.agent.isStopped = true;
            }
        }
    }


    public void Exit(Enemy enemy)
    {
        enemy.delayTimer.StopTimer();
        enemy.EndAttack();
        enemy.weapon.OnAttack -= () => AttackExecuted(enemy);

        if (!enemy.isLaunching) enemy.agent.isStopped = false;
    }
}

public class BossAttackingState : IEnemyState
{
    private float attackStartTime;
    private float rotationResumeTime;
    private int bulletsFired = 0;

    public void Enter(Enemy enemy)
    {
        enemy.delayTimer.SetTimer(enemy.attackStartDelay, enemy.StartAttack);

        enemy.weapon.OnAttack += () => AttackExecuted(enemy);
        attackStartTime = Time.time;
    }

    private void AttackExecuted(Enemy enemy)
    {
        bulletsFired++;
        if (bulletsFired >= enemy.bulletsPerBurst)
        {
            bulletsFired = 0;
            enemy.EndAttack();

            enemy.delayTimer.SetTimer(enemy.attackStartDelay, enemy.StartAttack);
        }
    }

    public void Execute(Enemy enemy)
    {
    }

    public void Exit(Enemy enemy)
    {
        enemy.EndAttack();
        enemy.delayTimer.StopTimer();
        enemy.weapon.OnAttack -= () => AttackExecuted(enemy);
    }
}