using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Linq;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectCarrier), typeof(NavMeshAgent))]
public class ClerkController : MonoBehaviour
{
    [SerializeField] [ReadOnly]
    private Define.ClerkState currentState;

    private NavMeshAgent agent;
    private Animator anim;
    private ObjectCarrier carrier;
    private float moveSpeedMultiplier = 1f;
    private Transform currentMoveTarget;
    private bool skipFsmUpdateSingleFrame = false;
    
    #region Const
    private const float NavMeshRadius = 0.25f;
    private const float NavMeshHeight = 1f;
    private const float NavMeshStopDistance = 1f;
    private const float NavMeshSpeed = 2f;
    private const float NaveMeshAngularSpeed = 360f;
    #endregion
    
    #region AnimParam
    private readonly int animParamMoveSpeed = Animator.StringToHash("MoveSpeed");
    #endregion
    
    #region Properties
    public Define.ClerkState CurrentState => currentState;
    public Transform CurrentMoveTarget
    {
        get => currentMoveTarget;
        private set
        {
            currentMoveTarget = value;
            if (currentMoveTarget != null)
            {
                agent.SetDestination(currentMoveTarget.position);
                skipFsmUpdateSingleFrame = true;
            }
        }
    }
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        carrier = GetComponent<ObjectCarrier>();
        
        agent.updateRotation = false;
    }

    private void Update()
    {
        anim.SetFloat(animParamMoveSpeed, agent.remainingDistance > agent.stoppingDistance ? agent.velocity.magnitude : 0f);
        OnStateUpdate();
    }

    private void OnValidate()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        agent.radius = NavMeshRadius;
        agent.height = NavMeshHeight;
        agent.stoppingDistance = NavMeshStopDistance;
        agent.speed = NavMeshSpeed;
        agent.angularSpeed = NaveMeshAngularSpeed;
    }

    public async UniTask Init()
    {
        await UniTask.Yield();
        SetState(Define.ClerkState.Idle);
    }

    public void GoToPoint()
    {
        
    }

    private void RotationUpdate()
    {
        if (agent.velocity.magnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void SetState(Define.ClerkState state)
    {
        OnStateExit();
        currentState = state;
        OnStateEnter();
    }

    private void OnStateEnter()
    {
        switch (currentState)
        {
            case Define.ClerkState.Idle:
                CurrentMoveTarget = null;
                // TODO: 할일이 없을 때 예외처리 (일정 시간 후 재시도)
                ClerkManager.Instance.FindJob(this);
                break;
            case Define.ClerkState.MoveToBurger:
                CurrentMoveTarget = StoreManager.Instance.FoodPlatformStack.transform;
                break;
            case Define.ClerkState.MoveToCounter:
                CurrentMoveTarget = StoreManager.Instance.CounterBurgerStack.transform;
                break;
            case Define.ClerkState.MoveToCleaning:
                CurrentMoveTarget = StoreManager.Instance.UnlockedTables.First(t => t.CurrentTableState == Define.TableState.Used).transform;
                break;
            case Define.ClerkState.MoveToDumpster:
                CurrentMoveTarget = StoreManager.Instance.Dumpster.transform;
                break;
        }
    }

    private void OnStateUpdate()
    {
        if (skipFsmUpdateSingleFrame)
        {
            skipFsmUpdateSingleFrame = false;
            return;
        }
        
        switch (currentState)
        {
            case Define.ClerkState.Idle:
                break;
            case Define.ClerkState.MoveToBurger:
                RotationUpdate();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    // TODO: carry능력만큼 들고가기
                    var burger = StoreManager.Instance.FoodPlatformStack.Pop();
                    carrier.PushCarryingObject(burger);
                    SetState(Define.ClerkState.MoveToCounter);
                }
                break;
            case Define.ClerkState.MoveToCounter:
                RotationUpdate();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    while (carrier.Count > 0)
                    {
                        StoreManager.Instance.CounterBurgerStack.Push(carrier.PopCarryingObject());   
                    }
                    SetState(Define.ClerkState.Idle);
                }
                break;
            case Define.ClerkState.MoveToCleaning:
                RotationUpdate();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    // TODO: null 체크
                    var dirtyTable = currentMoveTarget.GetComponent<TableSet>();
                    var trash = dirtyTable.GetTrash();
                    carrier.PushCarryingObject(trash);
                    SetState(Define.ClerkState.MoveToDumpster);
                }
                break;
            case Define.ClerkState.MoveToDumpster:
                RotationUpdate();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    var trash = carrier.PopCarryingObject();
                    currentMoveTarget.GetComponent<Dumpster>().PushTrashManually(trash);
                    SetState(Define.ClerkState.Idle);
                }
                break;
        }
    }

    private void OnStateExit()
    {
        switch (currentState)
        {
            case Define.ClerkState.Idle:
                break;
            case Define.ClerkState.MoveToBurger:
                break;
            case Define.ClerkState.MoveToCounter:
                break;
            case Define.ClerkState.MoveToCleaning:
                break;
            case Define.ClerkState.MoveToDumpster:
                break;
        }
    }
}
