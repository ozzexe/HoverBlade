using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;

    protected enum EnemyState { Patrol, Chase, Attack };
    protected EnemyState currentState = EnemyState.Patrol;

    public EnemyDataSO enemyDataSO;

    public Transform[] PatrolPoints;

    private int currentPatrolIndex = 0;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.speed = enemyDataSO.patrolSpeed;
        animator.SetFloat("Speed", enemyDataSO.patrolSpeed);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= enemyDataSO.detectionRange)
                {
                    currentState = EnemyState.Chase;
                    agent.speed = enemyDataSO.chaseSpeed;
                    animator.SetFloat("Speed", enemyDataSO.chaseSpeed);
                }
                break;

            case EnemyState.Chase:
                Chase();
                if (distanceToPlayer <= enemyDataSO.attackRange)
                {
                    currentState = EnemyState.Attack;
                    animator.SetTrigger("Attack");
                }
                else if (distanceToPlayer > enemyDataSO.detectionRange)
                {
                    currentState = EnemyState.Patrol;
                    agent.speed = enemyDataSO.patrolSpeed;
                    animator.SetFloat("Speed", enemyDataSO.patrolSpeed);
                }
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    protected virtual void Patrol()
    {
        if (PatrolPoints.Length == 0) return;

        agent.SetDestination(PatrolPoints[currentPatrolIndex].position);

        if (Vector3.Distance(transform.position, PatrolPoints[currentPatrolIndex].position) < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % PatrolPoints.Length;
        }
    }

    protected virtual void Chase()
    {
        agent.SetDestination(player.position);
    }

    protected abstract void Attack();
}