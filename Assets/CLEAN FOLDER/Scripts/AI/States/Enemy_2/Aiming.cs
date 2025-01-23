using UnityEngine;
using BeykozEdu.FSM;

public class Aiming : BaseState<EnemyAIData>
{
    private Transform transform;
    private Transform playerTransform;

    private static readonly int OnAim = Animator.StringToHash("OnAim");

    public override void OnEnter()
    {
        transform = StateData.Enemy;
        playerTransform = StateData.playerHealthComp.transform;
        StateData.EnemyNavMeshAgent.ResetPath();
        StateData.EnemyAnimator.SetTrigger(OnAim);
    }
    public override void OnUpdate()
    {
        LookAtPlayer();

        /* Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        float rotationSpeed = 60f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        */

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
        StateData.EnemyAnimator.ResetTrigger(OnAim);
    }

    private void LookAtPlayer()
    {
        Vector3 direction = StateData.playerHealthComp.transform.position - StateData.Enemy.position;
        direction.y = 0; // Dikey ekseni sýfýrla
        float rotationSpeed = 5f;
        StateData.Enemy.rotation = Quaternion.Slerp(StateData.Enemy.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
    }



}
