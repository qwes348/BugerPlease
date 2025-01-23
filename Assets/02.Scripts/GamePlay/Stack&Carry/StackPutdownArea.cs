using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class StackPutdownArea : MonoBehaviour
{
    [SerializeField]
    private ObjectStacker stacker;
    [SerializeField]
    private Define.StackableType canPutdownTypes;
    
    private PlayerActionArea actionArea;
    private CancellationTokenSource cts;

    private void Awake()
    {
        if(stacker == null)
            stacker = GetComponent<ObjectStacker>();
        
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.PickupPutdownTresholdTime);

        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) =>
        {
            cts = new CancellationTokenSource();
            await Putdown(pc.GetComponent<ObjectCarrier>());
        });
        actionArea.OnPlayerOutArea += pc =>
        {
            cts?.Cancel();
        };
    }

    private async UniTask Putdown(ObjectCarrier objectCarrier)
    {
        while (true)
        {
            if (cts.IsCancellationRequested)
                break;
            if ((objectCarrier.GetNextCarriableType() & canPutdownTypes) == 0)
                break;  // 내려놓을 수 없는 타입인 경우
            var carried = objectCarrier.PopCarryingObject();
            if (carried == null)
                break;  // 손에든거 다 내려놓은 경우
            
            // TODO: Stacker에 한도가 생기면 여기서 처리해야됨
            stacker.Push(carried);

            await UniTask.Yield();
        }
        
        cts.Dispose();
        cts = null;
    }
}
