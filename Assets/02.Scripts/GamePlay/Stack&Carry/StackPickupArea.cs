using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class StackPickupArea : MonoBehaviour
{
    [SerializeField]
    private ObjectStacker stacker;
    // 픽업 가능한 타입
    // 식사 테이블의 경우 음식도 놓고 쓰레기도 놓지만 쓰레기만 픽업 가능함
    [SerializeField]
    private Define.StackableType canPickupType = Define.StackableType.All;
    
    private PlayerActionArea actionArea;
    private CancellationTokenSource cts;

    private void Awake()
    {
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.PickupPutdownTresholdTime);
        
        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) =>
        {
            cts = new CancellationTokenSource();
            await Pickup(pc.GetComponent<ObjectCarrier>());
        });
        actionArea.OnPlayerOutArea += (PlayerController pc) =>
        {
            cts?.Cancel();
        };
    }

    private async UniTask Pickup(ObjectCarrier objectCarrier)
    {
        while (true)
        {
            if (stacker.Peek() != null && (stacker.Peek().MyStackableType & canPickupType) == 0)
                break;
            if (!objectCarrier.IsCanCarryMore)
            {
                cts.Dispose();
                cts = null;
                break;                
            }
            if (cts.IsCancellationRequested)
            {
                // 들고갈게 없어서 continue로 돌던 중 플레이어가 떠나면 여기로 들어올 것
                cts.Dispose();
                cts = null;
                return;
            }
            
            var obj = stacker.Pop();
            if (obj == null)
            {
                // 들고갈게 없으면 한프레임 쉬고 continue
                // continue를 하지 않으면 오브젝트가 새로 생성됐을 때 플레이어는 trigger영역에서 나갔다가 다시 들어와야됨
                // 연속적으로 생성되는 "음식"의 경우를 위한 처리임
                await UniTask.Yield();
                continue;
            }
            
            objectCarrier.PushCarryingObject(obj);
            await UniTask.Yield();
        }
    }
}
