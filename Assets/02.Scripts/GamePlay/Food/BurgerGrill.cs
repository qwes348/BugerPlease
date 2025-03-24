using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BurgerGrill : FoodFactory
{
    [SerializeField]
    private List<GameObject> patties;

    private const float animCoolTime = 3f;
    private CancellationTokenSource cts;

    protected override void Start()
    {
        base.Start();
        Managers.Game.onGameStateChanged += state =>
        {
            switch (state)
            {
                case Define.GameState.Running:
                    BurgerPattyAnim().Forget();
                    break;
                case Define.GameState.GameOver:
                    cts.Cancel();
                    break;
            }
        };
    }

    private async UniTask BurgerPattyAnim()
    {
        cts = new CancellationTokenSource();
        
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(animCoolTime), cancellationToken: cts.Token);
            if (cts.IsCancellationRequested)
                break;

            foreach (var patty in patties)
            {
                Sequence seq = DOTween.Sequence()
                    .Append(patty.transform.DOMoveY(patty.transform.position.y + 0.25f, 0.25f))
                    .Append(patty.transform.DOBlendableRotateBy(Vector3.forward * 180f, 0.2f))
                    .Append(patty.transform.DOMoveY(patty.transform.position.y, 0.25f));
            }
        }
        
        cts.Dispose();
    }
}
