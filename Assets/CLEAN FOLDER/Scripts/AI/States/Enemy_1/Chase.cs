using UnityEngine;
using BeykozEdu.FSM;

public class Chase : BaseState<EnemyAIData>
{
    private Transform transform;
    private Transform playerTransform;

    private static readonly int OnChase = Animator.StringToHash("OnChase");

    public override void OnEnter()
    {
        transform = StateData.Enemy;
        playerTransform = StateData.playerHealthComp.transform;
        StateData.EnemyNavMeshAgent.speed = StateData.AIDataSO.runSpeed;
        StateData.EnemyAnimator.SetTrigger(OnChase);
    }
    public override void OnUpdate()
    {
        StateData.EnemyNavMeshAgent.SetDestination(playerTransform.position);
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance < StateData.AIDataSO.chaseToAttackDistance)
        {
            StateMachineHandler.AddState(new Attack(), StateData);
        }
        else if (distance > StateData.AIDataSO.chaseToPatrolDistance)
        {
            StateMachineHandler.RemoveState();
        }

    }
    public override void OnExit()
    {
        StateData.EnemyNavMeshAgent.ResetPath();
        StateData.EnemyAnimator.ResetTrigger(OnChase);

    }




}
