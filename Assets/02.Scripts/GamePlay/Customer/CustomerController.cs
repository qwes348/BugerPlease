using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class CustomerController : MonoBehaviour
{
    [SerializeField][ReadOnly]
    private Define.CustomerState currentState;
    
    private NavMeshAgent agent;
    private Animator anim;
    private int wantBurgerCount;
    private GameObjectCarrier carrier;
    
    #region Properties
    public Define.CustomerState CurrentState => currentState;
    public TableSeat MyTableSeat { get; set; }
    #endregion
    
    #region Const
    private const float NavMeshRadius = 0.4f;
    private const float NavMeshHeight = 1f;
    private const float NavMeshStopDistance = 0.2f;
    private const float NavMeshSpeed = 2f;
    private const float NaveMeshAngularSpeed = 360f;
    #endregion
    
    #region AnimParam
    private readonly int animParamMoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int animParamIsSitting = Animator.StringToHash("IsSitting");
    #endregion
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        carrier = GetComponent<GameObjectCarrier>();
    }

    private void Update()
    {
        anim.SetFloat(animParamMoveSpeed, agent.remainingDistance > agent.stoppingDistance ? agent.velocity.magnitude : 0f);
        OnStateUpdate();
    }

    private void OnValidate()
    {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        agent.radius = NavMeshRadius;
        agent.height = NavMeshHeight;
        agent.stoppingDistance = NavMeshStopDistance;
        agent.speed = NavMeshSpeed;
        agent.angularSpeed = NaveMeshAngularSpeed;
    }

    public void Init(int wantBurgerCount)
    {
        this.wantBurgerCount = wantBurgerCount;
        SetState(Define.CustomerState.Waiting);
    }

    public void GoToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    private async UniTask Ordering()
    {
        // 주문대로 이동
        GoToPoint(StoreManager.Instance.CustomerLineController.OrderLinePoinnt.position);
        await UniTask.Yield();
        
        // 주문대로 이동할때까지 대기 & 버거가 준비될때까지 대기
        while (agent.remainingDistance > agent.stoppingDistance || StoreManager.Instance.BurgerStack.Count <= 0)
        {
            await UniTask.Yield();
        }

        var burger = StoreManager.Instance.BurgerStack.Pop();
        carrier.PushCarryingObject(burger.transform);
        SetState(Define.CustomerState.SeatWaiting);
    }

    // 자리를 지정받을때까지 대기
    private async UniTask WaitForSeatAssign()
    {
        while (true)
        {
            if (StoreManager.Instance.TryAssignSeat(this))
            {
                SetState(Define.CustomerState.Eating);
                break;
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }
    
    public void SetState(Define.CustomerState state)
    {
        OnStateExit();
        currentState = state;
        OnStateEnter();
    }

    private void OnStateEnter()
    {
        switch (currentState)
        {
            case Define.CustomerState.Waiting:
                break;
            case Define.CustomerState.Ordering:
                Ordering();
                break;
            case Define.CustomerState.SeatWaiting:
                WaitForSeatAssign();
                break;
            case Define.CustomerState.Eating:
                GoToPoint(MyTableSeat.transform.position);
                break;
            case Define.CustomerState.Exiting:
                break;
        }
    }
    
    private void OnStateUpdate()
    {
        switch (currentState)
        {
            case Define.CustomerState.Waiting:
                break;
            case Define.CustomerState.Ordering:
                break;
            case Define.CustomerState.Eating:
                break;
            case Define.CustomerState.Exiting:
                break;
        }
    }

    private void OnStateExit()
    {
        switch (currentState)
        {
            case Define.CustomerState.SeatWaiting:
                Debug.LogError("SeatWaitingExit");
                // 다음 손님을 주문대로 보내게함
                StoreManager.Instance.CustomerLineController.NextOrderRequest();
                break;
            case Define.CustomerState.Eating:
                MyTableSeat = null;
                break;
            case Define.CustomerState.Exiting:
                break;
        }
    }
}
