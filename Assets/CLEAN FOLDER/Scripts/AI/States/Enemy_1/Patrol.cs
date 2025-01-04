using UnityEngine;
using BeykozEdu.FSM;

public class Patrol : BaseState<EnemyAIData>
{
    private Transform transform;
    private Transform playerTransform;

    private static readonly int OnPatrol = Animator.StringToHash("OnPatrol");

    public override void OnEnter()
    {
        transform = StateData.Enemy;
        playerTransform = StateData.playerHealthComp.transform;
        SelectRandomWaypoint();
        StateData.EnemyNavMeshAgent.speed = StateData.AIDataSO.walkSpeed;
        StateData.EnemyAnimator.SetTrigger(OnPatrol);
    }

    public override void OnUpdate()
    {
        if (StateData.EnemyNavMeshAgent.remainingDistance<0.25f)
        {
            SelectRandomWaypoint();
        }

        if(StateData.Enemy.GetComponent<EnemyTypeOneController>() != null)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) < StateData.AIDataSO.patrolToChaseDistance)
            {
                StateMachineHandler.AddState(new Chase(), StateData);
            }
        }

        if (StateData.Enemy.GetComponent<EnemyTyoeTwoController>() != null)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) < StateData.AIDataSO.patrolToChaseDistance)
            {
                StateMachineHandler.AddState(new Aiming(), StateData);
            }
        }


    }

    public override void OnExit()
    {
        StateData.EnemyAnimator.ResetTrigger(OnPatrol);

    }

    private void SelectRandomWaypoint()
    {
        var newPoint = StateData.waypoints[Random.Range(0, (StateData.waypoints.Length) - 1)].position;
        StateData.EnemyNavMeshAgent.SetDestination(newPoint);
    }

}
