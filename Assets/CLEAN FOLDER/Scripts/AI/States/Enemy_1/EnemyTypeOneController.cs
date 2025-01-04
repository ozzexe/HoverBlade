using UnityEngine;
using UnityEngine.AI;
using BeykozEdu.FSM;
using BeykozEdu.FSM.Interfaces;

public class EnemyTypeOneController : EnemyAIController
{
    protected override void StatesType()
    {
        _stateMachineHandler.AddState(new Patrol(), aiData);
    }
}
