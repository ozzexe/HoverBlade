using UnityEngine;

public class BossController : EnemyAIController
{
    protected override void StatesType()
    {
        _stateMachineHandler.AddState(new Idle(), aiData);
    }

    
}
