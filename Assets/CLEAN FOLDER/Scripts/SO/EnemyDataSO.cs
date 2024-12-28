using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public float detectionRange;
    public float attackRange;
    public float patrolSpeed;
    public float chaseSpeed;
}
