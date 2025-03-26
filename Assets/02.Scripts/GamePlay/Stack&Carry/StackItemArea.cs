using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class StackItemArea : MonoBehaviour
{
    [SerializeField]
    private ObjectStacker stacker;
    
    private PlayerActionArea actionArea;
    private CancellationTokenSource cts;

    private void Awake()
    {
        if(stacker == null)
            stacker = GetComponent<ObjectStacker>();
        
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.GetItemTresholdTime);

        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) =>
        {
            cts = new CancellationTokenSource();
            await GetItems(pc);
        });
        actionArea.OnPlayerOutArea += pc =>
        {
            cts?.Cancel();
        };
    }

    // 기본적인 로직은 StackPickupArea와 비슷
    private async UniTask GetItems(PlayerController pc)
    {
        while (true)
        {
            if (cts.IsCancellationRequested)
            {
                cts.Dispose();
                cts = null;
                return;
            }
            
            var obj = stacker.Pop() as StackableItem;
            if (obj == null)
            {
                await UniTask.Yield();
                continue;
            }
            obj.OnGetItem();
            obj.JumpToPointAndClear(pc.transform.position).Forget();
            Managers.Audio.PlaySfx(Define.Sfx.Carry).Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
    }
}
