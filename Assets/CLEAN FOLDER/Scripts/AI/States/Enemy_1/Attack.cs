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
        

    }
    public override void OnUpdate()
    {
        LookAtPlayer();

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

            StateData.EnemyAnimator.SetTrigger(OnAttack);
            StateData.playerHealthComp.TakeDamage(5);
            await Task.Delay(TimeSpan.FromSeconds(4));
            
        }
    }
    private void LookAtPlayer()
    {
        Vector3 direction = StateData.playerHealthComp.transform.position - StateData.Enemy.position;
        direction.y = 0; // Dikey ekseni sýfýrla
        float rotationSpeed = 60f;
        StateData.Enemy.rotation = Quaternion.Slerp(StateData.Enemy.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
    }

}
