using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 점원을 소환하고 각 점원에게 명령을 내려줄 클래스
public class ClerkManager : StaticMono<ClerkManager>
{
    [SerializeField][ReadOnly]
    private List<ClerkController> activatedClerks = new List<ClerkController>();

    private void Start()
    {
        UpgradeManager.Instance.onClerkUpgrade += OnLevelUpClerkStat;
        Managers.Game.onGameStateChanged += OnGameStateChanged;
    }

    [Button]
    public async UniTask SpawnNewClerk()    
    {
        var newClerk = (await Managers.Pool.PopAsync("Prefab/Clerk")).GetComponent<ClerkController>();
        activatedClerks.Add(newClerk);
        newClerk.transform.position = StoreManager.Instance.TransformPoints.SpawnPoint.position;
        newClerk.gameObject.SetActive(true);
        await newClerk.Init();
    }

    public async UniTask FindJob(ClerkController clerk)
    {
        while (true)
        {
            if (Managers.Game.GameState != Define.GameState.Running)
                break;
            
            // 더러운 테이블이 있을 때
            if (StoreManager.instance.UnlockedTables.Any(t => t.CurrentTableState == Define.TableState.Used && t.currentCleaningClerk == null))
            {
                Debug.Log("점원 테이블 청소하러 이동");
                // 더러운 테이블로 이동 명령
                clerk.SetState(Define.ClerkState.MoveToCleaning);
                break;
            }
        
            // 생성된 버거가 있을 때
            if (StoreManager.instance.FoodPlatformStack.Count > 0)
            {
                Debug.Log("점원 버거 픽업하러 이동");
                // 버거스택으로 이동 명령
                clerk.SetState(Define.ClerkState.MoveToBurger);
                break;
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            Debug.Log("점원 할 일 못찾음");
        }
    }

    /// <summary>
    /// 점원 업그레이드
    /// </summary>
    /// <param name="statType"></param>
    public void OnLevelUpClerkStat(Define.UpgradeType statType)
    {
        switch (statType)
        {
            case Define.UpgradeType.MoveSpeed:
                foreach (ClerkController clerk in activatedClerks)
                {
                    clerk.UpdateMoveSpeed();
                }
                break;
            case Define.UpgradeType.CarryingCount:
                break;
            case Define.UpgradeType.HireClerk:
                SpawnNewClerk().Forget();
                break;
        }
    }

    public void OnGameStateChanged(Define.GameState gameState)
    {
        if (gameState != Define.GameState.GameOver)
            return;
        
        activatedClerks.ForEach(c => c.SetState(Define.ClerkState.Idle));
    }
}
