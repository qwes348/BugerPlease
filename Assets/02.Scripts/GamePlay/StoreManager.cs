using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
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
    
    [Header(("런타임 데이터"))]
    [SerializeField][ReadOnly]
    private List<TableSet> unlockedTables = new List<TableSet>();
    
    private CustomerLineController customerLineController;
    private Queue<TableSet> emptyTablesQueue = new Queue<TableSet>();
    private StoreTransformPoints transformPoints;
    private PlayerController player;
    
    #region Properties
    public CustomerLineController CustomerLineController => customerLineController;
    public ObjectStacker CounterBurgerStack => counterBurgerStack;
    public ObjectStacker FoodPlatformStack => foodPlatformStack;
    public StoreTransformPoints TransformPoints => transformPoints;
    public MoneyEffectController MoneyEffect => moneyEffect;
    public PlayerController Player => player;
    public IReadOnlyList<TableSet> UnlockedTables => unlockedTables;
    public Dumpster Dumpster => dumpster;
    #endregion
    
    #region Actions
    public Action<int> onMoneyChanged;
    #endregion

    private void Awake()
    {
        Managers.Game.Init();
        // 게임시작
        // TODO: 게임시작하는 부분 보완
        Managers.Game.GameState = Define.GameState.Running;
        
        customerLineController = GetComponent<CustomerLineController>();
        transformPoints = GetComponent<StoreTransformPoints>();
        player = FindAnyObjectByType<PlayerController>();
    }

    private void Start()
    {
        // TODO: 임시
        OnGameStart();
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

    [Button]
    public void AddMoneyDebug()
    {
        Managers.Game.MoneyAmount += 500;
    }
}
