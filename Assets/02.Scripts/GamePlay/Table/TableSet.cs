using NaughtyAttributes;
using System;
using System.Collections.Generic;
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
    
    public int EmptySeatsCount => seats.FindAll(s => s.CurrentSeatState == Define.SeatState.Empty).Count;
    
    public override void OnPurchase()
    {
        gameObject.SetActive(true);
        StoreManager.Instance.AddUnlockedTableSet(this);
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
}
