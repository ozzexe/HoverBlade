using UnityEngine;
using UnityEngine.AI;
using BeykozEdu.FSM;
using BeykozEdu.FSM.Interfaces;

//[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAIController : MonoBehaviour
{

    protected IStateMachineHandler<EnemyAIData> _stateMachineHandler;

    [SerializeField] protected EnemyAIData aiData;

    private void Start()
    {
        _stateMachineHandler = new StateMachineHandler<EnemyAIData>();
        StatesType();
        //_stateMachineHandler.AddState(new Patrol(), aiData);
    }

    protected abstract void StatesType();


    private void Update()
    {
        _stateMachineHandler.UpdateStates();
    }
}
