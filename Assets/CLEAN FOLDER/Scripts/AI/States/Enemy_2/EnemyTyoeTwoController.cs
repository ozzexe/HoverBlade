using UnityEngine;

public class EnemyTyoeTwoController : EnemyAIController
{
    protected override void StatesType()
    {
        _stateMachineHandler.AddState(new Patrol(), aiData);
    }

}
