using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoreManager : StaticMono<StoreManager>
{
    [SerializeField]
    private ObjectStacker counterBurgerStack;
    [SerializeField]
    private ObjectStacker moneyStack;
    
    [Header(("런타임 데이터"))]
    [SerializeField][ReadOnly]
    private List<TableSet> unlockedTables = new List<TableSet>();
    [SerializeField] [ReadOnly]
    private int moneyAmount;
    
    private CustomerLineController customerLineController;
    private Queue<TableSet> emptyTablesQueue = new Queue<TableSet>();
    private StoreTransformPoints transformPoints;
    
    #region Properties
    public CustomerLineController CustomerLineController => customerLineController;
    public ObjectStacker CounterBurgerStack => counterBurgerStack;
    public StoreTransformPoints TransformPoints => transformPoints;
    public int MoneyAmount
    {
        get => moneyAmount;
        set
        {
            moneyAmount = value;
            onMoneyChanged?.Invoke(moneyAmount);
        }
    }
    #endregion
    
    #region Actions
    public Action<int> onMoneyChanged;
    #endregion

    private void Awake()
    {
        moneyAmount = Define.StartingMoney;
        customerLineController = GetComponent<CustomerLineController>();
        transformPoints = GetComponent<StoreTransformPoints>();
    }

    private void Start()
    {
        // TODO: 임시
        OnGameStart();
    }

    public void OnGameStart()
    {
        customerLineController.NextOrderRequest().Forget();
        customerLineController.ContinuosSpawnCustomers().Forget();
    }

    public void AddUnlockedTableSet(TableSet tableSet)
    {
        unlockedTables.Add(tableSet);
        emptyTablesQueue.Enqueue(tableSet);
    }

    public void OnTableEmpty(TableSet table)
    {
        emptyTablesQueue.Enqueue(table);
    }

    // 고객이 true를 리턴받을때 까지 일정 간격으로 호출
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

    public async UniTask AddMoneyBundleToStack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var moneyBundle = (await Managers.Pool.PopAsync("Prefab/MoneyBundle")).GetComponent<Stackable>();
            moneyBundle.gameObject.SetActive(true);
            moneyStack.Push(moneyBundle);
        }
    }
}
