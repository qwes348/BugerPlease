using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BurgerGrill : FoodFactory
{
    [SerializeField]
    private List<GameObject> patties;

    private const float animCoolTime = 3f;

    protected override void Start()
    {
        base.Start();
        BurgerPattyAnim().Forget();
    }

    private async UniTask BurgerPattyAnim()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(animCoolTime));

            foreach (var patty in patties)
            {
                Sequence seq = DOTween.Sequence()
                    .Append(patty.transform.DOMoveY(patty.transform.position.y + 0.25f, 0.25f))
                    .Append(patty.transform.DOBlendableRotateBy(Vector3.forward * 180f, 0.2f))
                    .Append(patty.transform.DOMoveY(patty.transform.position.y, 0.25f));
            }
        }
    }
}
