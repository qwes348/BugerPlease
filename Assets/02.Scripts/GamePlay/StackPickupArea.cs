using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class StackPickupArea : MonoBehaviour
{
    [SerializeField]
    private ObjectStacker stacker;
    
    private PlayerActionArea actionArea;
    private CancellationTokenSource cts;

    private void Awake()
    {
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.PickupPutdownTresholdTime);
        
        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) =>
        {
            cts = new CancellationTokenSource();
            await Pickup(pc.GetComponent<GameObjectCarrier>());
        });
        actionArea.OnPlayerOutArea += (PlayerController pc) =>
        {
            cts?.Cancel();
        };
    }

    private async UniTask Pickup(GameObjectCarrier carrier)
    {
        while (true)
        {
            if (!carrier.IsCanCarryMore)
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
                await UniTask.Yield();
                continue;
            }
            
            carrier.PushCarryingObject(obj.transform);
            await UniTask.Yield();
        }
    }
}
