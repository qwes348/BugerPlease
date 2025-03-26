using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Linq;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(ObjectCarrier), typeof(NavMeshAgent))]
public class ClerkController : MonoBehaviour
{
    [SerializeField] [ReadOnly]
    private Define.ClerkState currentState;

    private NavMeshAgent agent;
    private Animator anim;
    private ObjectCarrier carrier;
    private Transform currentMoveTarget;
    private bool skipFsmUpdateSingleFrame = false;
    
    #region Const
    private const float NavMeshRadius = 0.05f;
    private const float NavMeshHeight = 1f;
    private const float NavMeshStopDistance = 0.3f;
    private const float NavMeshSpeed = 0.5f;
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
        agent.angularSpeed = NaveMeshAngularSpeed;
    }

    public async UniTask Init()
    {
        UpdateMoveSpeed();
        UpdateCarryingCapacity();
        await UniTask.Yield();
        SetState(Define.ClerkState.Idle);
    }

    private void RotationUpdate()
    {
        if (agent.velocity.magnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void UpdateMoveSpeed()
    {
        agent.speed = NavMeshSpeed * (UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.MoveSpeed) + 1);
    }

    public void UpdateCarryingCapacity()
    {
        carrier.CarryableCountAdder = UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.CarryingCount);
    }

    /// <summary>
    /// 현재 치우러 가고있는 테이블이 누군가에의해 치워졌을 때
    /// </summary>
    private void OnDirtyTableBeCleanByOthers()
    {
        if (currentMoveTarget == null)
            return;
        if (agent.remainingDistance <= agent.stoppingDistance)  // 이 조건이 true면 현재 직원 인스턴스가 가져간거일 확률이 높음
            return;
        var targetTable = currentMoveTarget.GetComponent<TableSet>();
        if (targetTable == null)
            return;
        
        targetTable.onAllTrashRemoved -= OnDirtyTableBeCleanByOthers;
        SetState(Define.ClerkState.Idle);
        
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
                ClerkManager.Instance.FindJob(this);
                break;
            case Define.ClerkState.MoveToBurger:
                CurrentMoveTarget = StoreManager.Instance.FoodPlatformStack.transform;
                break;
            case Define.ClerkState.MoveToCounter:
                CurrentMoveTarget = StoreManager.Instance.CounterBurgerStack.transform;
                break;
            case Define.ClerkState.MoveToCleaning:
                // 쓰레기가있고 치우러가는 직원이없는 테이블을 찾음
                var targetTable = StoreManager.Instance.UnlockedTables.First(t => t.CurrentTableState == Define.TableState.Used && t.currentCleaningClerk == null);
                targetTable.currentCleaningClerk = this;
                targetTable.onAllTrashRemoved += OnDirtyTableBeCleanByOthers;
                CurrentMoveTarget = targetTable.transform;
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
                    if (StoreManager.Instance.FoodPlatformStack.Count <= 0)
                    {
                        SetState(Define.ClerkState.Idle);
                        break;
                    }
                    
                    for (int i = 0; i < (UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.CarryingCount) + 1); i++)
                    {
                        var burger = StoreManager.Instance.FoodPlatformStack.Pop();
                        if (burger == null)
                            break;
                        carrier.PushCarryingObject(burger);
                    }
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
                    if (currentMoveTarget == null)
                    {
                        SetState(Define.ClerkState.Idle);
                        break;
                    }
                    var dirtyTable = currentMoveTarget.GetComponent<TableSet>();
                    var trash = dirtyTable.GetTrash();
                    if (trash == null)
                    {
                        SetState(Define.ClerkState.Idle);
                        break;
                    }
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
