using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableSet : PurchasableItem
{
    [SerializeField]
    private List<TableSeat> seats;
    
    [HorizontalLine]
    [Header("런타임 데이터")]
    [SerializeField][ReadOnly]
    private List<CustomerController> currentUsingCustomers;
    [SerializeField][ReadOnly]
    private Define.TableState currentTableState;

    private int arrivedCustomersCount;
    private ObjectStacker stacker;
    private PlayerActionArea actionArea;
    
    public Define.TableState CurrentTableState => currentTableState;
    public int EmptySeatsCount => seats.FindAll(s => s.CurrentSeatState == Define.SeatState.Empty).Count;
    
    public override void OnPurchased()
    {
        stacker = GetComponent<ObjectStacker>();
        actionArea = GetComponent<PlayerActionArea>();
        stacker.leftTopCornerPos = transform.position + Vector3.up * 0.5f;
            
        gameObject.SetActive(true);
        StoreManager.Instance.AddUnlockedTableSet(this);
        foreach (var st in seats)
        {
            st.MyTable = this;
        }
        
        Init();
    }

    private void Init()
    {
        actionArea.enabled = false;
        currentUsingCustomers.Clear();
        currentTableState = Define.TableState.Empty;
        seats.ForEach(seat => seat.SetSeatState(Define.SeatState.Empty));
        arrivedCustomersCount = 0;
    }

    public TableSeat AssignSeat(CustomerController customer)
    {
        var emptySeat = seats.Find(s => s.CurrentSeatState == Define.SeatState.Empty);
        if (emptySeat == null)
        {
            Debug.LogError("빈 자리 없음 !!");
            return null;
        }
        
        emptySeat.SetSeatState(Define.SeatState.Using);
        currentUsingCustomers.Add(customer);
        customer.MyTableSeat = emptySeat;
        return emptySeat;
    }

    public void OnCustomerArrived(CustomerController customer)
    {
        while (true)
        {
            var food = customer.ObjectCarrier.PopCarryingObject();
            if (food == null)
                break;
            stacker.Push(food);            
        }
        
        arrivedCustomersCount++;
        // 모든 좌석이 차면 식사 타이머 시작
        if (arrivedCustomersCount >= seats.Count)
        {
            EatingTimer().Forget();
        }
    }

    private async UniTask EatingTimer()
    {
        Debug.Log("식사 시작");
        int totalBurgerCount = currentUsingCustomers.Sum(customer => customer.WantBurgerCount);
        float eatingTime = Define.EatingTimePerBurger * totalBurgerCount;
        
        await UniTask.Delay(TimeSpan.FromSeconds(eatingTime));
        
        OnEatingEnd().Forget();
    }

    private async UniTask OnEatingEnd()
    {
        currentUsingCustomers.ForEach(customer => customer.SetState(Define.CustomerState.Exiting));
        currentTableState = Define.TableState.Used;
        stacker.Clear();

        var trash = (await Managers.Pool.PopAsync("Prefab/TrashSet")).GetComponent<Stackable>();
        stacker.Push(trash);
        trash.gameObject.SetActive(true);
        // 쓰레기를 가져갈 수 있게 플레이어 액션 에어리어 활성화
        actionArea.enabled = true;
        stacker.onEmpty += OnTrashRemoved;
        
        Debug.Log("식사 끝");
    }

    /// <summary>
    /// 테이블에 쓰레기가 치워지고 나서 호출<br/>
    /// 다시 사용 가능한 테이블이됨
    /// </summary>
    private void OnTrashRemoved()
    {
        stacker.onEmpty -= OnTrashRemoved;
        Init();
        
        // 빈 테이블 목록에 이 테이블을 추가
        StoreManager.Instance.OnTableEmpty(this);
    }

    // 직원이 치울때 호출하는 용도
    // 플레이어는 PlayerActionArea를 통해서 가져감
    public Stackable GetTrash()
    {
        if (currentTableState != Define.TableState.Used)
            return null;
        return stacker.Count <= 0 ? null : stacker.Pop();
    }
}
