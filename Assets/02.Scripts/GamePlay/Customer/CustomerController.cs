using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
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
    private ObjectCarrier objectCarrier;
    
    #region Properties
    public Define.CustomerState CurrentState => currentState;
    public int WantBurgerCount => wantBurgerCount;
    public TableSeat MyTableSeat { get; set; }
    public ObjectCarrier ObjectCarrier => objectCarrier;
    #endregion
    
    #region Const
    private const float NavMeshRadius = 0.05f;
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
        objectCarrier = GetComponent<ObjectCarrier>();

        agent.updateRotation = false;
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

    private void RotationUpdate()
    {
        if (agent.velocity.magnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private async UniTask Ordering()
    {
        GameUI.Instance.SpeechBubbleCanvas.ActiveBubble(this, Define.SpeechBubbleType.BurgerCount);
        // 주문대로 이동
        GoToPoint(StoreManager.Instance.TransformPoints.OrderLinePoint.position);
        await UniTask.Yield();
        
        // 주문대로 이동할때까지 대기 & 버거가 준비될때까지 대기
        while (agent.remainingDistance > agent.stoppingDistance || StoreManager.Instance.CounterBurgerStack.Count < wantBurgerCount)
        {
            await UniTask.Yield();
            if (Managers.Game.GameState != Define.GameState.Running)
                return;
        }
        for (int i = 0; i < wantBurgerCount; i++)
        {
            var burger = StoreManager.Instance.CounterBurgerStack.Pop();
            objectCarrier.PushCarryingObject(burger);
        }
        SetState(Define.CustomerState.SeatWaiting);
    }

    // 자리를 지정받을때까지 대기
    private async UniTask WaitForSeatAssign()
    {
        GameUI.Instance.SpeechBubbleCanvas.Clear();
        GameUI.Instance.SpeechBubbleCanvas.ActiveBubble(this, Define.SpeechBubbleType.NoSeat);
        while (true)
        {
            if (Managers.Game.GameState != Define.GameState.Running)
                return;
            
            if (StoreManager.Instance.TryAssignSeat(this))
            {
                GameUI.Instance.SpeechBubbleCanvas.Clear();
                SetState(Define.CustomerState.Eating);
                break;
            }
            
            // 1초마다 빈자리 요구
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }

    private async UniTask EatingSequence()
    {
        Managers.Game.AddScore(Define.Score_CustomerStartEating * wantBurgerCount);
        StoreManager.Instance.SoldCustomerCount++;
        StoreManager.Instance.SoldBurgerCount += wantBurgerCount;
     
        // 내 자리로 이동
        GoToPoint(MyTableSeat.transform.position);
        await UniTask.Yield();
        
        // 이동 완료까지 대기
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            if (Managers.Game.GameState != Define.GameState.Running)
                return;
            
            // 도착하기 전까지만 회전 업데이트
            RotationUpdate();
            await UniTask.Yield();
        }
        
        // 이동 완료를 테이블에 알림
        MyTableSeat.OnCustomerArrived(this);
        // 앉기 애니메이션 재생
        transform.rotation = MyTableSeat.transform.rotation;
        anim.SetBool(animParamIsSitting, true);
    }

    private async UniTask ExitingSequence()
    {
        StoreManager.Instance.AddMoneyBundleToStack(wantBurgerCount).Forget();
        anim.SetBool(animParamIsSitting, false);
        GoToPoint(StoreManager.Instance.TransformPoints.ExitPoint.position);
        await UniTask.Yield();

        while (agent.remainingDistance > agent.stoppingDistance)
        {
            if (Managers.Game.GameState != Define.GameState.Running)
                return;
            
            await UniTask.Yield();
        }
        
        Managers.Pool.Push(GetComponent<Poolable>());
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
                EatingSequence();
                break;
            case Define.CustomerState.Exiting:
                ExitingSequence();
                break;
        }
    }
    
    private void OnStateUpdate()
    {
        switch (currentState)
        {
            case Define.CustomerState.Waiting:
                RotationUpdate();
                break;
            case Define.CustomerState.Ordering:
                RotationUpdate();
                break;
            case Define.CustomerState.Eating:
                break;
            case Define.CustomerState.Exiting:
                RotationUpdate();
                break;
        }
    }

    private void OnStateExit()
    {
        switch (currentState)
        {
            case Define.CustomerState.SeatWaiting:
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
