using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class StackPutdownArea : MonoBehaviour
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
            await Putdown(pc.GetComponent<GameObjectCarrier>());
        });
        actionArea.OnPlayerOutArea += pc =>
        {
            cts?.Cancel();
        };
    }

    private async UniTask Putdown(GameObjectCarrier carrier)
    {
        while (true)
        {
            if (cts.IsCancellationRequested)
                break;
            var carried = carrier.PopCarryingObject();
            if (carried == null)
                break;  // 손에든거 다 내려놓은 경우
            
            // TODO: Stacker에 한도가 생기면 여기서 처리해야됨
            stacker.Push(carried.gameObject);

            await UniTask.Yield();
        }
        
        cts.Dispose();
        cts = null;
    }
}
