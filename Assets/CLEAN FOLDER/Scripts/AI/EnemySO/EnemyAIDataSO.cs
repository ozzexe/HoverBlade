using UnityEngine;

[CreateAssetMenu(menuName = "AI/Data/Enemy")]
public class EnemyAIDataSO : ScriptableObject
{
    public float patrolToChaseDistance = 5;
    public float chaseToAttackDistance = 1;
    public float chaseToPatrolDistance = 8;
    public float attackToChaseDistance = 2;

    public float walkSpeed = 1;
    public float runSpeed = 3;
}
