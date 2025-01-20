using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerActionArea : MonoBehaviour
{
    public Action<PlayerController> OnPlayerInArea;
    public Action<PlayerController> OnPlayerOutArea;

    private float thresholdTime;
    private float timer;
    private CancellationTokenSource cts;

    public void Init(float thresholdTime)
    {
        this.thresholdTime = thresholdTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (cts != null)
            return;
        
        cts = new CancellationTokenSource();
        WaitThresholdTime(other.GetComponent<PlayerController>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        // 선 딜레이 시간이 지나기 전에 플레이어가 영역에서 나감 -> 선 딜레이 대기 취소
        cts?.Cancel();
        OnPlayerOutArea?.Invoke(other.GetComponent<PlayerController>());
    }

    private async UniTask WaitThresholdTime(PlayerController pc)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(thresholdTime));

        if (cts.IsCancellationRequested)
        {
            cts.Dispose();
            cts = null;
            return;
        }
        
        OnPlayerInArea?.Invoke(pc);
        cts.Dispose();
        cts = null;
    }
}
