using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Serialization;

public class StoreManager : StaticMono<StoreManager>
{
    [SerializeField]
    private ObjectStacker counterBurgerStack;
    [SerializeField]
    private ObjectStacker foodPlatformStack;
    [SerializeField]
    private ObjectStacker moneyStack;
    [SerializeField]
    private MoneyEffectController moneyEffect;
    [SerializeField]
    private Dumpster dumpster;
    [SerializeField]
    private List<GameObject> activeAfterPurchaseTableset;
    
    [Header(("런타임 데이터"))]
    [SerializeField][ReadOnly]
    private List<TableSet> unlockedTables = new List<TableSet>();
    
    private CustomerLineController customerLineController;
    private Queue<TableSet> emptyTablesQueue = new Queue<TableSet>();
    private StoreTransformPoints transformPoints;
    private PlayerController player;
    
    private int soldCustomerCount;
    private int soldBurgerCount;
    private int totalSales;
    
    #region Properties
    public CustomerLineController CustomerLineController => customerLineController;
    public ObjectStacker CounterBurgerStack => counterBurgerStack;
    public ObjectStacker FoodPlatformStack => foodPlatformStack;
    public StoreTransformPoints TransformPoints => transformPoints;
    public MoneyEffectController MoneyEffect => moneyEffect;
    public PlayerController Player => player;
    public IReadOnlyList<TableSet> UnlockedTables => unlockedTables;
    public Dumpster Dumpster => dumpster;
    public int SoldCustomerCount { get; set; }
    public int SoldBurgerCount { get; set; }
    public int TotalSales => totalSales;
    #endregion
    
    #region Actions
    public Action<int> onMoneyChanged;
    #endregion

    private void Awake()
    {
        Managers.Game.onGameStateChanged += OnGameStateChanged;
        Managers.Game.onMoneyAdded += OnMoneyAdded;
        
        customerLineController = GetComponent<CustomerLineController>();
        transformPoints = GetComponent<StoreTransformPoints>();
        player = FindAnyObjectByType<PlayerController>();
        
        activeAfterPurchaseTableset.ForEach(go => go.SetActive(false));
    }

    private void Update()
    {
        if (Managers.Game.GameState != Define.GameState.Running)
            return;
        
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            AddMoneyDebug();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Managers.Game.onGameStateChanged -= OnGameStateChanged;
        Managers.Game.onMoneyAdded -= OnMoneyAdded;
    }

    private void OnGameStateChanged(Define.GameState state)
    {
        switch (state)
        {
            case Define.GameState.Running:
                OnGameStart();
                break;
            case Define.GameState.GameOver:
                break;
        }
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

        if (unlockedTables.Count == 1)
        {
            activeAfterPurchaseTableset.ForEach(go => go.SetActive(true));
        }
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

    [Button]
    public void AddMoneyDebug()
    {
        Managers.Game.MoneyAmount += 500;
    }

    private void OnMoneyAdded(int value)
    {
        if (value < 0)
            return;
        totalSales += value;
    }
}
