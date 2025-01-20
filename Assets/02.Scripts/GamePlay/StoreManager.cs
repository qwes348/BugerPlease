using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoreManager : StaticMono<StoreManager>
{
    [SerializeField]
    private ObjectStacker burgerStack;
    
    [Header(("런타임 데이터"))][ReadOnly]
    [SerializeField]
    private List<TableSet> unlockedTables = new List<TableSet>();
    [SerializeField] [ReadOnly]
    private Queue<TableSet> emptyTablesQueue = new Queue<TableSet>();
    
    private CustomerLineController customerLineController;
    
    #region Properties
    public CustomerLineController CustomerLineController => customerLineController;
    public ObjectStacker BurgerStack => burgerStack;
    #endregion

    private void Awake()
    {
        customerLineController = GetComponent<CustomerLineController>();
    }

    private void Start()
    {
        // TODO: 임시
        OnGameStart();
    }

    public void OnGameStart()
    {
        customerLineController.NextOrderRequest();
    }

    public void AddUnlockedTableSet(TableSet tableSet)
    {
        unlockedTables.Add(tableSet);
        emptyTablesQueue.Enqueue(tableSet);
    }

    public void OnTableEmpty(TableSet table)
    {
        // TODO: 제일 앞 고객 테이블로 보내기
        emptyTablesQueue.Enqueue(table);
    }

    public bool TryAssignSeat(CustomerController customer)
    {
        if (emptyTablesQueue.Count == 0)
            return false;

        var table = emptyTablesQueue.Peek();
        table.AssignSeat(customer);
        if(table.EmptySeatsCount == 0)
            emptyTablesQueue.Dequeue();

        return true;
    }
}
