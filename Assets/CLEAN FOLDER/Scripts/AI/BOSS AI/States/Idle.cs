using BeykozEdu.FSM;
using UnityEngine;

public class Idle : BaseState<EnemyAIData>
{
    private static readonly int OnIdle = Animator.StringToHash("OnIdle");
    public override void OnEnter()
    {
        StateData.EnemyAnimator.SetTrigger(OnIdle);
        Debug.Log("Idle Giri� yapt�m");
    }

    public override void OnExit()
    {
        StateData.EnemyAnimator.ResetTrigger(OnIdle);
        Debug.Log("Idle ��k�� yapt�m");
    }

    public override void OnUpdate()
    {
        Debug.Log("Idle �al��t�rmaya devam ediyorum");


        if (Vector3.Distance(StateData.Enemy.position, StateData.playerHealthComp.transform.position) < 10f)
        {
            StateMachineHandler.AddState(new Jumping(), StateData);
        }
    }


}
