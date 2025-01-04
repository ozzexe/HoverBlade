using UnityEngine;
using System.Threading.Tasks;
using BeykozEdu.FSM;
using System;

public class Attack : BaseState<EnemyAIData>
{
    private bool _isPlayerInRange;

    private static readonly int OnAttack = Animator.StringToHash("OnAttack");

    public override void OnEnter()
    {
        _isPlayerInRange = true;
        Damage();
        StateData.EnemyAnimator.SetTrigger(OnAttack);

    }
    public override void OnUpdate()
    {
        if (StateData.playerHealthComp == null)
            return;

        float distance = Vector3.Distance(StateData.playerHealthComp.transform.position, StateData.Enemy.position);
        if (distance > StateData.AIDataSO.attackToChaseDistance)
        {
            StateMachineHandler.RemoveState();
        }
    }
    public override void OnExit()
    {
        _isPlayerInRange = false;
        StateData.EnemyAnimator.ResetTrigger(OnAttack);
    }

    private async void Damage()
    {
        while (_isPlayerInRange)
        {
            if (StateData.playerHealthComp == null)
                break;

            StateData.playerHealthComp.TakeDamage(10);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
