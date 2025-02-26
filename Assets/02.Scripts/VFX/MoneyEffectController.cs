using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;

public class MoneyEffectController : MonoBehaviour
{
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Vector3 endPosition;

    private CancellationTokenSource cts;
    private const float IntervalTime = 0.15f;

    public void StartEffect(Vector3 startPosition, Vector3 endPosition)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        cts = new CancellationTokenSource();
        EffectLoopTask().Forget();
    }

    private async UniTask EffectLoopTask()
    {
        while (true)
        {
            if (cts.IsCancellationRequested)
            {
                cts.Dispose();
                cts = null;
                break;
            }
            
            var money = (await Managers.Pool.PopAsync("Prefab/MoneyBundleDeco")).gameObject;
            money.transform.position = startPosition;
            money.gameObject.SetActive(true);
            money.transform.DOJump(endPosition, 1f, 1, 0.1f).OnComplete(() => Managers.Pool.Push(money.GetComponent<Poolable>()));
            
            await UniTask.Delay(TimeSpan.FromSeconds(IntervalTime), cancellationToken: cts.Token);
        }
    }

    public void StopEffect()
    {
        cts?.Cancel();
    }
}
