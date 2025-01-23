using UnityEngine;
using UnityEngine.AI;
using BeykozEdu.FSM.Interfaces;

[System.Serializable]
public class EnemyAIData : IBaseStateData
{
    public HealthComponent playerHealthComp;
    public Transform Player;
    public Transform Enemy;
    public NavMeshAgent EnemyNavMeshAgent;
    public Animator EnemyAnimator;
    public Transform[] waypoints;
    
    

    public EnemyAIDataSO AIDataSO;
}
