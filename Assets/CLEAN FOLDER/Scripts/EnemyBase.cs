using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;

    protected enum EnemyState { Patrol, Chase, Attack };
    protected EnemyState currentState = EnemyState.Patrol;

    public float DetectionRange;
    public float AttackRange;
    public float PatrolSpeed;
    public float ChaseSpeed;

    public Transform[] PatrolPoints;

    private int currentPatrolIndex = 0;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.speed = PatrolSpeed;
        animator.SetFloat("Speed", PatrolSpeed);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= DetectionRange)
                {
                    currentState = EnemyState.Chase;
                    agent.speed = ChaseSpeed;
                    animator.SetFloat("Speed", ChaseSpeed);
                }
                break;

            case EnemyState.Chase:
                Chase();
                if (distanceToPlayer <= AttackRange)
                {
                    currentState = EnemyState.Attack;
                    animator.SetTrigger("Attack");
                }
                else if (distanceToPlayer > DetectionRange)
                {
                    currentState = EnemyState.Patrol;
                    agent.speed = PatrolSpeed;
                    animator.SetFloat("Speed", PatrolSpeed);
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
